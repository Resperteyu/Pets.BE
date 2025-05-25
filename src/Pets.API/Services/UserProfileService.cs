using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Pets.API.Responses;
using Pets.API.Responses.Dtos;
using Pets.Db; // Added Pets.Db for PetsDbContext
using Pets.Db.Models;
using System;
using System.Linq; // Added for FirstOrDefaultAsync
using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetUserProfile(string userId);
        Task<UserProfileUpdateResult> UpdateUserProfile(string userId, UserProfileDto userProfileDto);
        Task<bool> DeleteUserProfile(string userId);
    }

    public class UserProfileService : IUserProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGeocodingService _geocodingService;
        private readonly IMapper _mapper;
        private readonly PetsDbContext _context; // Added PetsDbContext

        public UserProfileService(UserManager<ApplicationUser> userManager, IMapper mapper,
            IGeocodingService geocodingService, PetsDbContext context) // Added PetsDbContext to constructor
        {
            _userManager = userManager;
            _mapper = mapper;
            _geocodingService = geocodingService;
            _context = context; // Assign PetsDbContext
        }

        public async Task<UserProfileDto> GetUserProfile(string userId)
        {
            // Move to a common place
            var user = await _userManager.Users
            .Include(u => u.Address)
                .ThenInclude(a => a.Country)
            .Include(u => u.Address)
                .ThenInclude(a => a.Location)
            .Include(u => u.UserProfileInfo)
            .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            return _mapper.Map<UserProfileDto>(user);
        }

        public async Task<UserProfileUpdateResult> UpdateUserProfile(string userId, UserProfileDto userProfileDto)
        {
            if (!Guid.TryParse(userId, out Guid userGuid))
            {
                // Consider logging this error: Invalid userId format
                return new UserProfileUpdateResult { Success = false, Errors = new[] { new IdentityError { Code = "InvalidUserIdFormat", Description = "User ID format is invalid." } } };
            }

            var userProfileInfo = await _context.UserProfileInfo.FirstOrDefaultAsync(x => x.ApplicationUserId == userGuid);

            if (userProfileInfo == null)
            {
                // If userProfileInfo is null, return Success = false as per corrected instruction.
                return new UserProfileUpdateResult { Success = false, Errors = new[] { new IdentityError { Code = "NotFound", Description = "User profile not found." } } };
            }

            // Update properties from DTO
            userProfileInfo.FirstName = userProfileDto.FirstName;
            userProfileInfo.LastName = userProfileDto.LastName;
            userProfileInfo.PhoneNumber = userProfileDto.PhoneNumber;
            // Note: Other fields from UserProfileDto (UserName, Email, Address) are not part of UserProfileInfo
            // and thus are not updated here as per the specific instructions to use _context.UserProfileInfo.

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                // Log the exception (ex)
                return new UserProfileUpdateResult { Success = false, Errors = new[] { new IdentityError { Code = "DbUpdateError", Description = "Error saving profile to database." } } };
            }

            // Create a new UserProfileDto from the updated userProfileInfo.
            // This DTO will only contain fields present in UserProfileInfo.
            var updatedDto = new UserProfileDto
            {
                // Assuming UserProfileDto.Id is string representation of Guid
                Id = userProfileInfo.ApplicationUserId.ToString(), 
                FirstName = userProfileInfo.FirstName,
                LastName = userProfileInfo.LastName,
                PhoneNumber = userProfileInfo.PhoneNumber,
                // UserName, Email, Address will be null/default as they are not in userProfileInfo
            };

            return new UserProfileUpdateResult { Success = true, UpdatedProfile = updatedDto };
        }

        // Cascade deletion?
        public async Task<bool> DeleteUserProfile(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return false;
            }

            var result = await _userManager.DeleteAsync(user);
            return result.Succeeded;
        }
    }
}
