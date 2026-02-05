using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Pets.Db;
using AutoMapper;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Pets.API.ComponentTest;

// Boots the API with an in-memory DB + test auth scheme
public class TestWebAppFactory : WebApplicationFactory<Pets.API.Startup>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Test");

        // Override configuration for tests
        builder.ConfigureAppConfiguration((context, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JWT:Secret"] = "ThisIsAVeryLongSecretKeyForJWTTokenGenerationThatIsAtLeast256BitsLong",
                ["JWT:ValidIssuer"] = "www.petshub.net",
                ["JWT:ValidAudience"] = "https://localhost:5001/",
                ["JWT:TokenValidityInMinutes"] = "10",
                ["JWT:RefreshTokenValidityInDays"] = "7"
            });
        });

        builder.ConfigureServices(services =>
        {
            // Replace DbContext with InMemory provider used by the app
            services.RemoveAll(typeof(DbContextOptions<PetsDbContext>));
            services.AddDbContext<PetsDbContext>(options =>
                options.UseInMemoryDatabase("PetsTestsShared")); // Shared DB for all tests in this factory instance

            // Remove existing authentication and add test authentication
            services.RemoveAll(typeof(IAuthenticationService));
            services.RemoveAll(typeof(IAuthenticationHandlerProvider));
            services.RemoveAll(typeof(IAuthenticationSchemeProvider));
            services.AddAuthentication(TestAuthHandler.Scheme)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });

            // Override AutoMapper registration to avoid scanning all assemblies in test host
            services.RemoveAll(typeof(AutoMapper.IConfigurationProvider));
            services.RemoveAll(typeof(IMapper));
            var mapperAssemblies = new[]
            {
                typeof(Pets.API.Startup).Assembly,
                typeof(PetsDbContext).Assembly
            };
            services.AddAutoMapper(mapperAssemblies);

            // Replace external email sender with a fake
            services.RemoveAll(typeof(IEmailSender));
            services.AddSingleton<IEmailSender, FakeEmailSender>();
            services.AddSingleton<IEmailInbox>(sp => (FakeEmailSender)sp.GetRequiredService<IEmailSender>());
        });
        
        // Ensure database is created and seeded
        builder.ConfigureServices(services =>
        {
            var serviceProvider = services.BuildServiceProvider();
            using var scope = serviceProvider.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<PetsDbContext>();
            db.Database.EnsureCreated();
            
            // Seed roles using RoleManager
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
            SeedTestRoles(roleManager).GetAwaiter().GetResult();
        });
    }
    
    private static async Task SeedTestRoles(RoleManager<IdentityRole<Guid>> roleManager)
    {
        var roles = new[] { "PetOwner", "Administrator", "User" };
        
        foreach (var roleName in roles)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>(roleName));
                Console.WriteLine($"Created role: {roleName}");
            }
            else
            {
                Console.WriteLine($"Role already exists: {roleName}");
            }
        }
    }
}
