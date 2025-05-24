using AutoMapper;
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
    public class PetTypeServiceTests
    {
        private readonly IMapper _mapper;
        private readonly IPetTypeService _petTypeService;
        private readonly PetsDbContext _context;

        public PetTypeServiceTests()
        {
            var factory = new TestDbContextFactory();
            _context = factory.CreateDbContext();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
            _mapper = mapperConfig.CreateMapper();
            _petTypeService = new PetTypeService(_context, _mapper);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfCountryDto()
        {
            // Arrange
            var petTypes = new List<PetType>
            {
                new PetType { Id = 1, Name = "Cats" },
                new PetType { Id = 2, Name = "Dogs" }
            };
            await _context.PetTypes.AddRangeAsync(petTypes);
            await _context.SaveChangesAsync();

            // Act
            var result = await _petTypeService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<PetTypeDto>>(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, t => t.Id == 1 && t.Name == "Cats"); // TODO: Plural?
            Assert.Contains(result, t => t.Id == 2 && t.Name == "Dogs");
        }
    }
}
