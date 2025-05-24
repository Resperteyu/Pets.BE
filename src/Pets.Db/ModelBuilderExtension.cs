using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pets.Db.Models;
using System.Globalization;

namespace Pets.Db
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            var seedDataDirectory = Path.Combine(baseDirectory, "SeedData");

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                IgnoreBlankLines = false,
                HeaderValidated = null,
                MissingFieldFound = null,
            };

            modelBuilder.Entity<Country>().HasData(LoadCsvData<Country>(Path.Combine(seedDataDirectory, "EuropeanCountries.csv"), config));
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(LoadCsvData<IdentityRole<Guid>>(Path.Combine(seedDataDirectory, "Roles.csv"), config));
        }

        private static T[] LoadCsvData<T>(string filePath, CsvConfiguration config)
        {
            using (var reader = new StreamReader(filePath))
            using (var csv = new CsvReader(reader, config))
            {
                return csv.GetRecords<T>().ToArray();
            }
        }
    }
}
