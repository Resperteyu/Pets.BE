using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Pets.API.Responses;
using Pets.API.Responses.Dtos;
using Pets.Db.Models;
using System;
using System.Threading.Tasks;

namespace Pets.API.Services
{
    public interface IUserProfileService
    {
        Task<UserProfileDto> GetUserProfile(string userId);
        Task<UserProfileUpdateResult> UpdateUserProfile(string userId, JsonPatchDocument<UserProfileDto> patchDocument);
        Task<bool> DeleteUserProfile(string userId);
    }

    public class UserProfileService : IUserProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGeocodingService _geocodingService;
        private readonly IMapper _mapper;

        public UserProfileService(UserManager<ApplicationUser> userManager, IMapper mapper,
            IGeocodingService geocodingService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _geocodingService = geocodingService;
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

        public async Task<UserProfileUpdateResult> UpdateUserProfile(string userId, JsonPatchDocument<UserProfileDto> patchDocument)
        {
            var user = await _userManager.Users
            .Include(u => u.Address)
                .ThenInclude(a => a.Country)
            .Include(u => u.Address)
                .ThenInclude(a => a.Location)
            .Include(u => u.UserProfileInfo)
            .FirstOrDefaultAsync(u => u.Id.ToString() == userId);

            if (user == null)
            {
                return new UserProfileUpdateResult { Success = false };
            }

            var userProfile = _mapper.Map<UserProfileDto>(user);
            patchDocument.ApplyTo(userProfile);

            // Validate!

            if (AddressIsUpdated(patchDocument))
            {
                var newLocation = await _geocodingService.CalculateLocationAsync(userProfile.Address);
                userProfile.Address.Location = _mapper.Map<LocationDto>(newLocation);
            }

            _mapper.Map(userProfile, user);
            var result = await _userManager.UpdateAsync(user);

            return new UserProfileUpdateResult
            {
                Success = result.Succeeded,
                UpdatedProfile = result.Succeeded ? userProfile : null,
                Errors = result.Errors
            };
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
        private bool AddressIsUpdated(JsonPatchDocument<UserProfileDto> patchDocument)
        {
            foreach (var operation in patchDocument.Operations)
            {
                if (operation.path.StartsWith("/Address", StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
