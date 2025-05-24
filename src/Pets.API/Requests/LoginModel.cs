using System.ComponentModel.DataAnnotations;

namespace Pets.API.Requests
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
