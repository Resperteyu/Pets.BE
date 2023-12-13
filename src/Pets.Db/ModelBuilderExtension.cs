using CsvHelper;
using CsvHelper.Configuration;
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

            modelBuilder.Entity<Sex>().HasData(LoadCsvData<Sex>(Path.Combine(seedDataDirectory, "Sex.csv"), config));
            modelBuilder.Entity<Country>().HasData(LoadCsvData<Country>(Path.Combine(seedDataDirectory, "Country.csv"), config));
            modelBuilder.Entity<PetType>().HasData(LoadCsvData<PetType>(Path.Combine(seedDataDirectory, "PetType.csv"), config));
            modelBuilder.Entity<PetBreed>().HasData(LoadCsvData<PetBreed>(Path.Combine(seedDataDirectory, "PetBreed.csv"), config));
            modelBuilder.Entity<MateRequestState>().HasData(LoadCsvData<MateRequestState>(Path.Combine(seedDataDirectory, "MateRequestState.csv"), config));
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
