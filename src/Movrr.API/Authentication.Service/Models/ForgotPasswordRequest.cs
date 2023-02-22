using System.ComponentModel.DataAnnotations;

namespace Movrr.API.Authentication.Service.Models
{
  public class ForgotPasswordRequest
  {
    [Required]
    [EmailAddress]
    public string Email { get; set; }
  }
}