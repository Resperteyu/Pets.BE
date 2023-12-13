using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using Pets.Db.Models;
using System.Threading.Tasks;

namespace Pets.API.Controllers
{

    // TODO Document proper response codes
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserProfileController : ControllerBase
    {
        private readonly IUserProfileService _userProfileService;
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileController(UserManager<ApplicationUser> userManager, 
            IUserProfileService userProfileService)
        {
            _userManager = userManager;
            _userProfileService = userProfileService;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = _userManager.GetUserId(User);
            var userProfile = await _userProfileService.GetUserProfile(userId);

            return userProfile == null ? NotFound() : Ok(userProfile);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateProfile([FromBody] JsonPatchDocument<UserProfileDto> patchDocument)
        {
            var userId = _userManager.GetUserId(User);
            var updateResult = await _userProfileService.UpdateUserProfile(userId, patchDocument);

            if (!updateResult.Success)
            {
                return updateResult.Errors != null ? BadRequest(updateResult.Errors) : NotFound();
            }

            return Ok(updateResult.UpdatedProfile);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            var userId = _userManager.GetUserId(User);
            var success = await _userProfileService.DeleteUserProfile(userId);

            return success ? NoContent() : NotFound();
        }
    }
}
