using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Pets.API.Responses;
using Pets.Db.Models;
using System.Threading.Tasks;

namespace Pets.API.Controllers
{
    [Authorize]
    [Route("[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileController(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<ActionResult<UserProfileModel>> GetProfile()
        {
           var user = await _userManager.GetUserAsync(User);
           if (user == null)
            {
                return NotFound();
            }

            return new UserProfileModel
            {
                //Name = user.UserName, // Decide what to do with it
                Email = user.Email,
                //EmailVerified = user.EmailConfirmed, // For tick?
                FirstName = user.FirstName,
                LastName = user.LastName,
                Telephone = user.PhoneNumber,
                //TelephoneVerified = user.PhoneNumberConfirmed
                CountryName = user.Country?.Name,
                //Location = user.Location.GeoLocation // TODO: Align with ui
            };
        }

        [HttpPatch]
        public async Task<IActionResult> UpdateProfile([FromBody] JsonPatchDocument<UserProfileModel> patchDocument)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var userProfile = new UserProfileModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Telephone = user.PhoneNumber
            };

            patchDocument.ApplyTo(userProfile, ModelState);

            if (!TryValidateModel(userProfile))
            {
                return BadRequest(ModelState);
            }

            user.Email = userProfile.Email;
            user.FirstName = userProfile.FirstName;
            user.LastName = userProfile.LastName;
            user.PhoneNumber = userProfile.Telephone;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return Ok(userProfile);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }

            return NoContent();
        }
    }
}
