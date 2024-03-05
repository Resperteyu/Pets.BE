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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
             options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"), x => x.UseNetTopologySuite()));
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
                            new string[] {}
                    }
                 });
            });

            services.AddSendGrid(options =>
                options.ApiKey = Configuration.GetValue<string>("SendGridApiKey") ?? throw new Exception("The 'SendGridApiKey' is not configured")
            );

            services.AddTransient<StubbedHttpClientHandler>();
            services.AddHttpClient<IGeocodingService, GoogleGeocodingService>()
                .ConfigurePrimaryHttpMessageHandler(serviceProvider =>
                    {
                        var featureManager = serviceProvider.GetRequiredService<IFeatureManager>();
                        bool isStubbed = !featureManager.IsEnabledAsync("UseGeocodingApi").GetAwaiter().GetResult(); // Dynamically to use async? Just a ff on the service?

                        if (isStubbed)
                        {
                            return serviceProvider.GetRequiredService<StubbedHttpClientHandler>();
                        }

                        return new HttpClientHandler();
                    });

            services.AddSingleton<IEmailSender, SendGridEmailSender>();
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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, PetsDbContext context)
        {
            context.Database.Migrate();

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
