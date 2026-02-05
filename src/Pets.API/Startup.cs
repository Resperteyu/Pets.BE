using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Pets.API.Clients;
using Pets.API.Config;
using Pets.API.Helpers;
using Pets.API.Middleware;
using Pets.API.Services;
using Pets.API.Settings;
using Pets.Db;
using Pets.Db.Models;
using SendGrid.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Net.Http;
using System.Text.Json;

namespace Pets.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<PetsDbContext>(options =>
             options.UseNpgsql(Configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite()));
            services.AddHealthChecks();

            services.AddControllers(options =>
            {
                options.InputFormatters.Insert(0, GetJsonPatchInputFormatter());
            })
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.AddFeatureManagement();
            services.AddMemoryCache();
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddPetsAuth(Configuration);

            IdentityBuilder builder = services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.User.RequireUniqueEmail = true;
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.ClaimsIdentity.UserIdClaimType = "Id";
            });

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole<Guid>), builder.Services);
            builder.AddUserManager<UserManager<ApplicationUser>>()
                .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
                .AddSignInManager<SignInManager<ApplicationUser>>()
                .AddEntityFrameworkStores<PetsDbContext>()
                .AddTokenProvider<DataProtectorTokenProvider<ApplicationUser>>(TokenOptions.DefaultProvider);

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pets API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = JwtBearerDefaults.AuthenticationScheme,
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter '{token}' to authenticate, no need to enter 'Bearer'"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                    {
                        new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                        []
                    }
                 });
            });

            services.AddSingleton<IEmailSender, ConsoleEmailSender>();
            services.AddSingleton<EmailService>();

            services.AddScoped<ISexService, SexService>();
            services.AddScoped<IMateRequestStateService, MateRequestStateService>();
            services.AddScoped<IPetTypeService, PetTypeService>();
            services.AddScoped<IPetBreedService, PetBreedService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IPetProfileService, PetProfileService>();
            services.AddScoped<IMateRequestService, MateRequestService>();
            services.AddScoped<IUserProfileService, UserProfileService>();
            services.AddScoped<ILitterService, LitterService>();
            services.AddScoped<IServiceTypeService, ServiceTypeService>();
            services.AddScoped<IServiceOfferService, ServiceOfferService>();
            services.AddSingleton<IImageStorageService, ImageStorageService>();
            services.AddSingleton<IMateRequestStateChangeValidator, MateRequestStateChangeValidator>();
            services.AddSingleton<IFirebaseClient, FirebaseClient>();
            services.AddSingleton<IChatService, ChatService>();
            services.AddHttpClient<IGeocodingService, GoogleGeocodingService>();

            services.AddOptions<BlobStorageSettings>().Configure(options =>
            {
                Configuration.Bind("BlobStorage", options);
            });
            services.AddOptions<WebSiteSettings>().Configure(options =>
            {
                Configuration.Bind("WebSite", options);
            });
            services.AddOptions<FirestoreDbSettings>().Configure(options =>
            {
                Configuration.Bind("FirestoreDb", options);
            });

            services.AddApplicationInsightsTelemetry();

            // CORS policy for frontend
            var allowedOrigins = Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
                ?? new[] { "http://localhost:3000", "http://127.0.0.1:3000" };
            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend",
                    builder =>
                    {
                        builder.WithOrigins(allowedOrigins)
                               .AllowAnyHeader()
                               .AllowAnyMethod()
                               .AllowCredentials();
                    });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PetsDbContext context)
        {
            // Only run migrations for non-test environments and when using a relational database
            if (!env.IsEnvironment("Test") && context.Database.IsRelational())
            {
                try
                {
                    // Check if there are pending migrations before running
                    var pendingMigrations = context.Database.GetPendingMigrations().ToList();
                    if (pendingMigrations.Any())
                    {
                        var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogInformation("Applying {Count} pending migration(s): {Migrations}", 
                            pendingMigrations.Count, string.Join(", ", pendingMigrations));
                        context.Database.Migrate();
                    }
                    else
                    {
                        var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
                        logger.LogInformation("Database is up to date - no pending migrations.");
                    }
                }
                catch (Exception ex) when (ex.Message.Contains("already exists") || ex.Message.Contains("42P07"))
                {
                    // Tables exist but migration history might be out of sync
                    // This is common in local dev when DB was created manually or partially migrated
                    var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogWarning(ex, 
                        "Migration failed because tables already exist. " +
                        "If this is expected, ensure migration history is synced. " +
                        "To fix: Run 'dotnet ef database update --project ../Pets.Db/Pets.Db.csproj' manually.");
                    
                    // In development, continue; in production, this should be fixed
                    if (!env.IsDevelopment())
                    {
                        throw new InvalidOperationException(
                            "Database migration failed. Tables exist but migration history is out of sync. " +
                            "Please sync migrations manually.", ex);
                    }
                }
                catch (Exception ex)
                {
                    var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
                    logger.LogError(ex, "Migration failed with unexpected error.");
                    
                    // In production, fail fast; in development, log and continue
                    if (!env.IsDevelopment())
                    {
                        throw;
                    }
                }
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseRouting();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseMiddleware<ErrorHandlerMiddleware>();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", async context => await context.Response.WriteAsync("There is nothing here, this is an API."));
                endpoints.MapGet("/robots933456.txt", async context => await context.Response.WriteAsync($"User-agent: *{Environment.NewLine}Disallow: /"));
                endpoints.MapHealthChecks("/health");
                endpoints.MapHealthChecks("/health/detail", new HealthCheckOptions
                {
                    ResponseWriter = async (context, report) =>
                    {
                        var content = new
                        {
                            Status = report.Status.ToString(),
                            Results = report.Entries.ToDictionary(e => e.Key, e => new
                            {
                                Status = e.Value.Status.ToString(),
                                e.Value.Description,
                                e.Value.Duration
                            }),
                            report.TotalDuration
                        };

                        context.Response.ContentType = "application/json";
                        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(content, new JsonSerializerOptions { WriteIndented = true }));
                    }
                });

                endpoints.MapHealthChecks("/health/report", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }

        private static NewtonsoftJsonPatchInputFormatter GetJsonPatchInputFormatter()
        {
            var builder = new ServiceCollection()
                .AddLogging()
                .AddMvc()
                .AddNewtonsoftJson()
                .Services.BuildServiceProvider();

            return builder
                .GetRequiredService<IOptions<MvcOptions>>()
                .Value
                .InputFormatters
                .OfType<NewtonsoftJsonPatchInputFormatter>()
                .First();
        }
    }
}
