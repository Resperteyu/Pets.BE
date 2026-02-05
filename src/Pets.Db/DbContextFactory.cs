using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pets.Db
{
    public class DbContextFactory : IDesignTimeDbContextFactory<PetsDbContext>
    {
        public PetsDbContext CreateDbContext(string[] args)
        {
            // When running migrations, current directory is Pets.API/src/Pets.API
            // That's where appsettings.Development.json is located
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PetsDbContext>();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"),
                x => x.UseNetTopologySuite());

            return new PetsDbContext(optionsBuilder.Options);

        }
    }
}
