using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Pets.API.Responses.Dtos;
using Pets.API.Services;
using Pets.Db.Models;
using System;
using System.Threading.Tasks;

namespace Pets.API.Controllers
{

    // TODO Document proper response codes
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class UserProfileController(
        UserManager<ApplicationUser> userManager,
        IUserProfileService userProfileService,
        IPetProfileService petProfileService)
        : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<UserProfileDto>> GetProfile()
        {
            var userId = userManager.GetUserId(User);
            var userProfile = await userProfileService.GetUserProfile(userId);

            return userProfile == null ? NotFound() : Ok(userProfile);
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateProfile([FromBody] JsonPatchDocument<UserProfileDto> patchDocument)
        {
            var userId = userManager.GetUserId(User);
            var updateResult = await userProfileService.UpdateUserProfile(userId, patchDocument);

            if (!updateResult.Success)
            {
                return updateResult.Errors != null ? BadRequest(updateResult.Errors) : NotFound();
            }

            return Ok(updateResult.UpdatedProfile);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            var userId = userManager.GetUserId(User);
            var success = await userProfileService.DeleteUserProfile(userId);

            return success ? NoContent() : NotFound();
        }

        [HttpGet("{id:Guid}/view")]
        public async Task<ActionResult<ViewUserProfileDto>> GetProfile(Guid id)
        {
            var userProfile = await userProfileService.GetUserProfile(id.ToString());

            if (userProfile == null)
                return NotFound();

            var viewUserProfileDto = new ViewUserProfileDto
            {
                Id = id,
                UserName = userProfile.UserName,
                Pets = await petProfileService.GetPetsView(id)
            };

            return Ok(viewUserProfileDto);
        }
    }
}
