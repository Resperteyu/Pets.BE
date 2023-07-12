using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
                Name = user.UserName,
                Email = user.Email,
            };
        }
    }
}
