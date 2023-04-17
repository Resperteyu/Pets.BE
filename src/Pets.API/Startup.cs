using AutoMapper;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Pets.API.Authentication.Service;
using Pets.API.Email.Service;
using Pets.API.Middleware;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Pets.Db;

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
      services.AddDbContext<DbContext>();
      services.AddHealthChecks();
      services.AddControllersWithViews();
      services.AddMemoryCache();
      services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Pets API", Version = "v1" });
      });

      var emailConfig = Configuration
        .GetSection("EmailConfiguration")
        .Get<EmailConfiguration>();
      emailConfig.Password = Environment.GetEnvironmentVariable("EmailPassword");
      services.AddSingleton(emailConfig);
      services.AddScoped<IEmailService, EmailService>();
      services.AddScoped<IAccountService, AccountService>();

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
      }

      app.UseStaticFiles();
      app.UseHttpsRedirection();

      app.UseSwagger();

      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Pets.API V1");
        c.RoutePrefix = string.Empty;
      });

      app.UseRouting();

      app.UseAuthorization();

      app.UseMiddleware<ErrorHandlerMiddleware>();
      app.UseMiddleware<JwtMiddleware>();

      app.UseEndpoints(endpoints =>
      {
        endpoints.MapHealthChecks("/health");
        endpoints.MapControllerRoute(
          name: "default",
          pattern: "{controller=Home}/{action=Index}/{id?}"
        );
      });
    }
  }
}
