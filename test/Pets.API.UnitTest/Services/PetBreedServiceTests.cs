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
using Microsoft.EntityFrameworkCore.Internal;
using Pets.Db.Models;

namespace Pets.API.UnitTest.Services
{
    public class PetBreedServiceTests
    {
        private readonly IMapper _mapper;
        private readonly IPetBreedService _petBreedService;
        private readonly PetsDbContext _context;

        public PetBreedServiceTests()
        {
            var factory = new TestDbContextFactory();
            _context = factory.CreateDbContext();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
            _mapper = mapperConfig.CreateMapper();
            _petBreedService = new PetBreedService(_context, _mapper);
        }

        [Fact]
        public async Task GetAll_ReturnsListOfCountryDto()
        {
            // Arrange
            var petBreeds = new List<PetBreed>
            {
                new PetBreed { TypeId = 1, Title = "Bambino" },
                new PetBreed { TypeId = 2, Title = "Border Collie" }
            };
            await _context.PetBreeds.AddRangeAsync(petBreeds);
            await _context.SaveChangesAsync();

            // Act
            var result = await _petBreedService.GetAll();

            // Assert
            Assert.NotNull(result);
            Assert.IsType<List<PetBreedDto>>(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, b => b.TypeId == 1 && b.Title == "Bambino");
            Assert.Contains(result, b => b.TypeId == 2 && b.Title == "Border Collie");
        }
    }
}
