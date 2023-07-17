using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pets.API.Services;
using Pets.API.Responses.Dtos;
using Pets.Db;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Pets.API.Helpers;
using Pets.Db.Models;

namespace Pets.API.UnitTest.Services
{
    public class CountryServiceTests
    {
        private readonly IMapper _mapper;
        private readonly ICountryService _countryService;
        private readonly PetsDbContext _context;

        public CountryServiceTests()
        {
            var factory = new TestDbContextFactory();
            _context = factory.CreateDbContext();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
            _mapper = mapperConfig.CreateMapper();
            _countryService = new CountryService(_context, _mapper);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfCountryDto()
        {
            // Arrange
            var countries = new List<Country>
            {
                new Country { Code = "GB", Name = "United Kingdom", DialCode = "44" }, // TODO: DialCode not in dto?
                new Country { Code = "DE", Name = "Germany", DialCode = "88" },
            };
            await _context.Countries.AddRangeAsync(countries);
            await _context.SaveChangesAsync();

            // Act
            var result = await _countryService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<CountryDto>>(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.CountryCode == "GB" && c.Name == "United Kingdom");
            Assert.Contains(result, c => c.CountryCode == "DE" && c.Name == "Germany");
        }
    }
}
