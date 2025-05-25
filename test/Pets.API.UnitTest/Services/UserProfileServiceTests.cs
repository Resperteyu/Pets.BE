using Xunit;
using Moq;
using AutoMapper;
using Pets.API.Services;
using Pets.Db.Models;
using Pets.API.Responses.Dtos;
using Pets.API.Responses;
using Pets.Db;
using Pets.API.Helpers; // For AutoMapperProfile
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore; // Required for InMemory database options if TestDbContextFactory doesn't abstract it fully

namespace Pets.API.UnitTest.Services
{
    // Helper class to mock UserManager if not already available in the project
    // Based on https://github.com/dotnet/aspnetcore/blob/main/src/Identity/testshared/MockHelpers.cs
    public static class MockUserManager
    {
        public static Mock<UserManager<TUser>> Create<TUser>(IUserStore<TUser> store = null) where TUser : class
        {
            store = store ?? new Mock<IUserStore<TUser>>().Object;
            var options = new Mock<Microsoft.Extensions.Options.IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            idOptions.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            idOptions.Lockout.MaxFailedAccessAttempts = 5;
            idOptions.Lockout.AllowedForNewUsers = true;

            options.Setup(o => o.Value).Returns(idOptions);
            var userValidators = new List<IUserValidator<TUser>>();
            var passwordValidators = new List<IPasswordValidator<TUser>>();
            var userManager = new Mock<UserManager<TUser>>(store, options.Object,
                new Mock<IPasswordHasher<TUser>>().Object,
                userValidators, passwordValidators,
                new Mock<ILookupNormalizer>().Object,
                new Mock<IdentityErrorDescriber>().Object,
                new Mock<IServiceProvider>().Object,
                new Mock<Microsoft.Extensions.Logging.ILogger<UserManager<TUser>>>().Object);
            return userManager;
        }
    }

    public class UserProfileServiceTests
    {
        private readonly PetsDbContext _context;
        private readonly UserProfileService _userProfileService;
        private readonly IMapper _mapper;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly Mock<IGeocodingService> _mockGeocodingService;

        public UserProfileServiceTests()
        {
            var factory = new TestDbContextFactory();
            _context = factory.CreateDbContext();

            var mapperConfig = new MapperConfiguration(cfg => cfg.AddProfile(new AutoMapperProfile()));
            _mapper = mapperConfig.CreateMapper();

            _mockUserManager = MockUserManager.Create<ApplicationUser>();
            _mockGeocodingService = new Mock<IGeocodingService>();

            _userProfileService = new UserProfileService(
                _mockUserManager.Object,
                _mapper,
                _mockGeocodingService.Object,
                _context
            );
        }

        [Fact]
        public async Task UpdateUserProfile_WhenProfileExists_ReturnsSuccessAndUpdateProfile()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var existingProfileInfo = new UserProfileInfo
            {
                ApplicationUserId = userId,
                FirstName = "OriginalFirst",
                LastName = "OriginalLast",
                PhoneNumber = "1234567890"
            };
            _context.UserProfileInfo.Add(existingProfileInfo);
            await _context.SaveChangesAsync();

            var updateDto = new UserProfileDto
            {
                Id = userId.ToString(),
                FirstName = "UpdatedFirst",
                LastName = "UpdatedLast",
                PhoneNumber = "0987654321"
            };

            // Act
            var result = await _userProfileService.UpdateUserProfile(userId.ToString(), updateDto);

            // Assert
            Assert.True(result.Success);
            Assert.NotNull(result.UpdatedProfile);
            Assert.Equal(updateDto.FirstName, result.UpdatedProfile.FirstName);
            Assert.Equal(updateDto.LastName, result.UpdatedProfile.LastName);
            Assert.Equal(updateDto.PhoneNumber, result.UpdatedProfile.PhoneNumber);
            Assert.Equal(userId.ToString(), result.UpdatedProfile.Id);

            var dbProfile = await _context.UserProfileInfo.FindAsync(userId);
            Assert.NotNull(dbProfile);
            Assert.Equal("UpdatedFirst", dbProfile.FirstName);
            Assert.Equal("UpdatedLast", dbProfile.LastName);
            Assert.Equal("0987654321", dbProfile.PhoneNumber);
        }

        [Fact]
        public async Task UpdateUserProfile_WhenProfileNotFound_ReturnsFailure()
        {
            // Arrange
            var userId = Guid.NewGuid().ToString();
            var updateDto = new UserProfileDto
            {
                Id = userId,
                FirstName = "TestFirst",
                LastName = "TestLast",
                PhoneNumber = "1231231234"
            };

            // Act
            var result = await _userProfileService.UpdateUserProfile(userId, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.UpdatedProfile);
            Assert.NotNull(result.Errors);
            Assert.Contains(result.Errors, e => e.Code == "NotFound");
        }

        [Fact]
        public async Task UpdateUserProfile_WithInvalidUserIdFormat_ReturnsFailure()
        {
            // Arrange
            var invalidUserId = "not-a-guid";
            var updateDto = new UserProfileDto
            {
                Id = invalidUserId,
                FirstName = "TestFirst",
                LastName = "TestLast"
            };

            // Act
            var result = await _userProfileService.UpdateUserProfile(invalidUserId, updateDto);

            // Assert
            Assert.False(result.Success);
            Assert.Null(result.UpdatedProfile);
            Assert.NotNull(result.Errors);
            Assert.Contains(result.Errors, e => e.Code == "InvalidUserIdFormat");
        }
    }
}
