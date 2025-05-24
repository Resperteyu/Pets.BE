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
    public class SexServiceTests
    {
        private readonly IMapper _mapper;
        private readonly ISexService _sexService;
        private readonly PetsDbContext _context;

        public SexServiceTests()
        {
            var factory = new TestDbContextFactory();
            _context = factory.CreateDbContext();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
            _mapper = mapperConfig.CreateMapper();
            _sexService = new SexService(_context, _mapper);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfCountryDto()
        {
            // Arrange
            var sexes = new List<Sex>
            {
                new Sex { Id = 1, Title = "Male"},
                new Sex {Id = 2, Title = "Female"},
            };
            await _context.Sexes.AddRangeAsync(sexes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _sexService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<SexDto>>(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, s => s.Id == 1 && s.Title == "Male");
            Assert.Contains(result, s => s.Id == 2 && s.Title == "Female");
        }
    }
}
