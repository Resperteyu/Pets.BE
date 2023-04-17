using System.ComponentModel.DataAnnotations;

namespace Pets.API.Authentication.Service.Models
{
  public class AuthenticateRequest
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
  }
}