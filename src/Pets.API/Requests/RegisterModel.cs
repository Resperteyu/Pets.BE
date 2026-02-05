using System.ComponentModel.DataAnnotations;
using Pets.API.Responses.Dtos;

namespace Pets.API.Requests   
{
    public class RegisterModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;

        [Required]
        public string Username { get; set; } = null!;

        [Required]
        public string Password { get; set; } = null!;

        [Required]
        public string Role { get; set; } = null!;

        [Required]
        public string FirstName { get; set; } = null!;

        [Required]
        public string LastName { get; set; } = null!;

        public string? Telephone { get; set; }

        [Required]
        public AddressDto Address { get; set; } = null!;
    }
}