using System.ComponentModel.DataAnnotations;

namespace Pets.API.Authentication.Service.Models
{
  public class ForgotPasswordRequest
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}