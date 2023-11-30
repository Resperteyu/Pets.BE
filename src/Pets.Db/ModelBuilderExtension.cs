using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Pets.Db.Models;
using System.Globalization;
using System.Reflection;

namespace Pets.Db
{
    public static class ModelBuilderExtensions
    {
        public static void Seed(this ModelBuilder modelBuilder)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();

            var config = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                IgnoreBlankLines = false,
                HeaderValidated = null,
                MissingFieldFound = null,
            };

            string resourceName = "Pets.Db.SeedData.Sex.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (TextReader textReader = new StreamReader(stream))
                {
                    using (CsvReader csvReader = new CsvReader(textReader, config))
                    {
                        var sex = csvReader.GetRecords<Sex>().ToArray();
                        modelBuilder.Entity<Sex>().HasData(sex);
                    }
                }
            }

            resourceName = "Pets.Db.SeedData.Country.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (TextReader textReader = new StreamReader(stream))
                {
                    using (CsvReader csvReader = new CsvReader(textReader, config))
                    {
                        var countries = csvReader.GetRecords<Country>().ToArray();
                        modelBuilder.Entity<Country>().HasData(countries);
                    }
                }
            }

            resourceName = "Pets.Db.SeedData.PetType.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (TextReader textReader = new StreamReader(stream))
                {
                    using (CsvReader csvReader = new CsvReader(textReader, config))
                    {
                        var petTypes = csvReader.GetRecords<PetType>().ToArray();
                        modelBuilder.Entity<PetType>().HasData(petTypes);
                    }
                }
            }

            resourceName = "Pets.Db.SeedData.PetBreed.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (TextReader textReader = new StreamReader(stream))
                {
                    using (CsvReader csvReader = new CsvReader(textReader, config))
                    {
                        var petBreeds = csvReader.GetRecords<PetBreed>().ToArray();
                        modelBuilder.Entity<PetBreed>().HasData(petBreeds);
                    }
                }
            }

            resourceName = "Pets.Db.SeedData.MateRequestState.csv";
            using (Stream stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (TextReader textReader = new StreamReader(stream))
                {
                    using (CsvReader csvReader = new CsvReader(textReader, config))
                    {
                        var mateRequestStates = csvReader.GetRecords<MateRequestState>().ToArray();
                        modelBuilder.Entity<MateRequestState>().HasData(mateRequestStates);
                    }
                }
            }
        }
    }
}
