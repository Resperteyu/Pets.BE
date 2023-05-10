using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pets.Db;

namespace Pets.API.UnitTest
{
    public class TestDbContextFactory : IDbContextFactory<PetsDbContext>
    {
        private readonly DbContextOptions<PetsDbContext> _options;

        public TestDbContextFactory(string databaseName = "InMemoryTestPetsDb")
        {
            var serviceProvider = new ServiceCollection()
                .AddEntityFrameworkInMemoryDatabase()
                .AddEntityFrameworkSqlServerNetTopologySuite()
                .BuildServiceProvider();

            var builder = new DbContextOptionsBuilder<PetsDbContext>();
            builder.UseInMemoryDatabase(databaseName)
                   .UseInternalServiceProvider(serviceProvider);

            _options = builder.Options;
        }

        public PetsDbContext CreateDbContext()
        {
            return new PetsDbContext(_options);
        }
    }
}