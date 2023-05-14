using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Pets.Db
{
    public class DbContextFactory : IDesignTimeDbContextFactory<PetsDbContext>
    {
        public PetsDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory().ToLower())
                .AddJsonFile("appsettings.json", true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<PetsDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                x => x.UseNetTopologySuite());

            return new PetsDbContext(optionsBuilder.Options);

        }
    }
}
