using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Pets.API.Config;
using Pets.API.Middleware;
using Pets.API.Services;
using Pets.Db;
using Pets.Db.Models;
using SendGrid.Extensions.DependencyInjection;
using System;
using System.Linq;
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

            services.AddControllers();
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

            builder = new IdentityBuilder(builder.UserType, typeof(IdentityRole), builder.Services);
            builder.AddUserManager<UserManager<ApplicationUser>>()
                .AddUserManager<UserManager<ApplicationUser>>()
                .AddRoleManager<RoleManager<IdentityRole>>()
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
            services.AddTransient<IEmailSender, SendGridEmailSender>();

            services.AddScoped<ISexService, SexService>();
            services.AddScoped<IPetTypeService, PetTypeService>();
            services.AddScoped<IPetBreedService, PetBreedService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<IPetProfileService, PetProfileService>();

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
                        await context.Response.WriteAsync(JsonSerializer.Serialize(content, new JsonSerializerOptions { WriteIndented = true }));
                    }
                });

                endpoints.MapHealthChecks("/health/report", new HealthCheckOptions()
                {
                    Predicate = _ => true,
                    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                });
            });
        }
    }
}
