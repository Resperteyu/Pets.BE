using System.ComponentModel.DataAnnotations;

namespace Pets.API.Requests   
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        //TODO: Do we want username?
        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;
    }
}