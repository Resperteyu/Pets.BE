using Xunit;
using Moq;
using Pets.API.Controllers;
using Pets.API.Services;
using Pets.Db.Models;
using Pets.API.Responses.Dtos;
using Pets.API.Responses; // For UserProfileUpdateResult
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;

// Assuming MockUserManager is accessible, e.g., if it's in a shared test utilities file or UserProfileServiceTests.cs is compiled alongside.
// If not, it might need to be redefined or moved.
// For simplicity, if it's in UserProfileServiceTests.cs, and these are part of the same test project, it should be fine.
// Alternatively, could use a more basic Moq setup for UserManager here if MockUserManager helper is not directly usable.

namespace Pets.API.UnitTest.Controllers
{
    public class UserProfileControllerTests
    {
        private readonly Mock<IUserProfileService> _mockUserProfileService;
        private readonly Mock<IPetProfileService> _mockPetProfileService; // Added as it's a constructor dependency
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly UserProfileController _controller;
        private readonly string _testUserId = "test-user-id-guid";

        public UserProfileControllerTests()
        {
            _mockUserProfileService = new Mock<IUserProfileService>();
            _mockPetProfileService = new Mock<IPetProfileService>(); // Initialize mock

            // Setup MockUserManager (assuming the helper is available or using basic Moq setup)
            _mockUserManager = Services.MockUserManager.Create<ApplicationUser>(); // Using the one from UserProfileServiceTests namespace
            
            _controller = new UserProfileController(
                _mockUserManager.Object,
                _mockUserProfileService.Object,
                _mockPetProfileService.Object // Pass the mock
            );

            // Setup ClaimsPrincipal for controller's User
            var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, _testUserId),
            }, "mock"));
            _controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = userClaims }
            };

            // Setup UserManager GetUserId to return the testUserId
            _mockUserManager.Setup(um => um.GetUserId(_controller.User)).Returns(_testUserId);
        }

        [Fact]
        public async Task UpdateProfile_WithValidDto_WhenServiceSucceeds_ReturnsOk()
        {
            // Arrange
            var userProfileDto = new UserProfileDto { Id = _testUserId, FirstName = "Test" };
            var serviceResult = new UserProfileUpdateResult 
            { 
                Success = true, 
                UpdatedProfile = userProfileDto 
            };
            _mockUserProfileService.Setup(s => s.UpdateUserProfile(_testUserId, userProfileDto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdateProfile(userProfileDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(userProfileDto, okResult.Value);
            _mockUserProfileService.Verify(s => s.UpdateUserProfile(_testUserId, userProfileDto), Times.Once);
        }

        [Fact]
        public async Task UpdateProfile_WhenServiceFailsWithErrors_ReturnsBadRequest()
        {
            // Arrange
            var userProfileDto = new UserProfileDto { Id = _testUserId, FirstName = "Test" };
            var errors = new List<IdentityError> { new IdentityError { Code = "Error", Description = "Service failed" } };
            var serviceResult = new UserProfileUpdateResult 
            { 
                Success = false, 
                Errors = errors 
            };
            _mockUserProfileService.Setup(s => s.UpdateUserProfile(_testUserId, userProfileDto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdateProfile(userProfileDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(errors, badRequestResult.Value);
        }

        [Fact]
        public async Task UpdateProfile_WhenServiceFailsWithoutErrors_ReturnsNotFound()
        {
            // This test case specifically checks the scenario where Success is false AND Errors is null
            // which leads to a NotFoundResult based on the controller's logic:
            // if (!updateResult.Success) { return updateResult.Errors != null ? BadRequest(updateResult.Errors) : NotFound(); }

            // Arrange
            var userProfileDto = new UserProfileDto { Id = _testUserId, FirstName = "Test" };
            var serviceResult = new UserProfileUpdateResult 
            { 
                Success = false, 
                Errors = null // Critical for this test case
            };
            _mockUserProfileService.Setup(s => s.UpdateUserProfile(_testUserId, userProfileDto))
                .ReturnsAsync(serviceResult);

            // Act
            var result = await _controller.UpdateProfile(userProfileDto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
