using AutoMapper;
using Pets.API.Helpers;
using Pets.API.Requests;
using Pets.API.Services;
using Pets.Db;
using Pets.Db.Models;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Pets.API.UnitTest.Services
{
    public class PetProfileServiceTests
    {
        private readonly IMapper _mapper;
        private readonly IPetProfileService _petProfileService;
        private readonly PetsDbContext _context;

        public PetProfileServiceTests()
        {
            var factory = new TestDbContextFactory();
            _context = factory.CreateDbContext();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
            _mapper = mapperConfig.CreateMapper();
            _petProfileService = new PetProfileService(_context, _mapper);
        }

        [Fact]
        public async Task GetByPetId_ReturnsPetProfileDto()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var breedId = 1;
            var petBreed = new PetBreed { Id = breedId, TypeId = 2, Title = "Border Collie" };
            await _context.PetBreeds.AddAsync(petBreed);
            byte sexId = 1;
            var sex = new Sex { Id = 1, Title = "Male" };
            await _context.Sexes.AddAsync(sex);
            var owner = new ApplicationUser
            {
                Id = ownerId,
                FirstName = "John",
                LastName = "Doe",
                //CountryCode = "UK",
                //LocationId = 1, //TODO: Why is it required on the DB and not in code?
                PhoneNumber = "123456789"
            };
            _context.Users.Add(owner);
            var petProfile = new PetProfile
            {
                Id = Guid.NewGuid(),
                Name = "Test Pet",
                OwnerId = ownerId,
                Owner = owner,
                BreedId = breedId,
                Breed = petBreed,
                SexId = sexId,
                Sex = sex,
                Description = "Test Description",
            };
            _context.PetProfiles.Add(petProfile);
            await _context.SaveChangesAsync();

            // Act

            var result = await _petProfileService.GetByPetId(petProfile.Id);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(petProfile.Id, result.Id);
            Assert.Equal(petProfile.Name, result.Name);
            Assert.Equal(petProfile.OwnerId, result.Owner.Id); // TODO: Assert more on owner?
            Assert.Equal(petProfile.BreedId, result.Breed.Id);
            Assert.Equal(petBreed.Title, result.Breed.Title);
            Assert.Equal(petProfile.SexId, result.Sex.Id);
            Assert.Equal(sex.Title, result.Sex.Title);
            Assert.Equal(petProfile.Description, result.Description);
        }


        [Fact]
        public async Task GetByPetId_ReturnsNullIfPetProfileNotFound()
        {
            // Arrange
            var petId = Guid.NewGuid();

            // Act
            var result = await _petProfileService.GetByPetId(petId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByOwnerId_ReturnsListOfPetProfileDto()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var breedId = 1;
            var petBreed = new PetBreed { Id = breedId, TypeId = 2, Title = "Border Collie" };
            await _context.PetBreeds.AddAsync(petBreed);
            byte sexId = 1;
            var sex = new Sex { Id = 1, Title = "Male" };
            await _context.Sexes.AddAsync(sex);
            var owner = new ApplicationUser
            {
                Id = ownerId,
                FirstName = "John",
                LastName = "Doe",
                //CountryCode = "UK",
                //LocationId = 1, //TODO: Why is it required on the DB and not in code?
                PhoneNumber = "123456789"
            };
            _context.Users.Add(owner);
            var petProfile1 = new PetProfile
            {
                Id = Guid.NewGuid(),
                Name = "Test Pet",
                OwnerId = ownerId,
                Owner = owner,
                BreedId = breedId,
                Breed = petBreed,
                SexId = sexId,
                Sex = sex,
                Description = "Test Description",
            };
            var petProfile2 = new PetProfile
            {
                Id = Guid.NewGuid(),
                Name = "Test Pet 2",
                OwnerId = ownerId,
                Owner = owner,
                BreedId = breedId,
                Breed = petBreed,
                SexId = sexId,
                Sex = sex,
                Description = "Test Description 2",
            };
            _context.PetProfiles.AddRange(petProfile1, petProfile2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _petProfileService.GetByOwnerId(ownerId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, p => p.Id == petProfile1.Id && p.Name == petProfile1.Name);
            Assert.Contains(result, p => p.Id == petProfile2.Id && p.Name == petProfile2.Name);
        }


        [Fact]
        public async Task CreatePet_ReturnsNewPetId()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var createPetRequest = new CreatePetRequest { Name = "Test Pet", Description = "Test Description" };

            // Act
            var result = await _petProfileService.CreatePet(createPetRequest, ownerId);

            // Assert
            var petProfile = await _context.PetProfiles.FindAsync(result);
            Assert.NotNull(petProfile);
            Assert.Equal(createPetRequest.Name, petProfile.Name);
            Assert.Equal(ownerId, petProfile.OwnerId);
        }

        [Fact]
        public async Task UpdatePet_UpdatesPetProfile()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var owner = new ApplicationUser
            {
                Id = ownerId,
                FirstName = "John",
                LastName = "Doe",
                //CountryCode = "UK",
                //LocationId = 1, //TODO: Why is it required on the DB and not in code?
                PhoneNumber = "123456789"
            };
            var breedId = 1;
            var petBreed = new PetBreed { Id = breedId, TypeId = 2, Title = "Border Collie" };
            byte sexId = 1;
            var sex = new Sex { Id = 1, Title = "Male" };
            await _context.Sexes.AddAsync(sex);
            var petProfile = new PetProfile
            {
                Id = Guid.NewGuid(),
                Name = "Test Pet",
                OwnerId = ownerId,
                Owner = owner,
                BreedId = breedId,
                Breed = petBreed,
                SexId = sexId,
                Sex = sex,
                Description = "Test Description",
            };
            _context.PetProfiles.Add(petProfile);
            await _context.SaveChangesAsync();
            var updatePetRequest = new UpdatePetRequest
            {
                Name = "Updated Pet",
                SexId = 2,
                BreedId = 2,
                DateOfBirth = new DateTime(2019, 1, 1),
                AvailableForBreeding = false,
                Description = "Updated description"
            };

            // Act
            await _petProfileService.UpdatePet(updatePetRequest, petProfile);

            // Assert
            var updatedPetProfile = await _context.PetProfiles.FindAsync(petProfile.Id);
            Assert.NotNull(updatedPetProfile);
            Assert.Equal(updatePetRequest.Name, updatedPetProfile.Name);
            Assert.Equal(updatePetRequest.SexId, updatedPetProfile.SexId);
            Assert.Equal(updatePetRequest.BreedId, updatedPetProfile.BreedId);
            Assert.Equal(updatePetRequest.DateOfBirth, updatedPetProfile.DateOfBirth);
            Assert.Equal(updatePetRequest.AvailableForBreeding, updatedPetProfile.AvailableForBreeding);
            Assert.Equal(updatePetRequest.Description, updatedPetProfile.Description);
        }

        [Fact]
        public async Task DeletePet_RemovesPetProfile()
        {
            // Arrange
            var ownerId = Guid.NewGuid();
            var owner = new ApplicationUser
            {
                Id = ownerId,
                FirstName = "John",
                LastName = "Doe",
                //CountryCode = "UK",
                //LocationId = 1, //TODO: Why is it required on the DB and not in code?
                PhoneNumber = "123456789"
            };
            var breedId = 1;
            var petBreed = new PetBreed { Id = breedId, TypeId = 2, Title = "Border Collie" };
            byte sexId = 1;
            var sex = new Sex { Id = 1, Title = "Male" };
            await _context.Sexes.AddAsync(sex);
            var petProfile = new PetProfile
            {
                Id = Guid.NewGuid(),
                Name = "Test Pet",
                OwnerId = ownerId,
                Owner = owner,
                BreedId = breedId,
                Breed = petBreed,
                SexId = sexId,
                Sex = sex,
                Description = "Test Description",
            };
            _context.PetProfiles.Add(petProfile);
            await _context.SaveChangesAsync();

            // Act
            await _petProfileService.DeletePet(petProfile);

            // Assert
            petProfile = await _context.PetProfiles.FindAsync(petProfile.Id);
            Assert.Null(petProfile);
        }
    }
}
