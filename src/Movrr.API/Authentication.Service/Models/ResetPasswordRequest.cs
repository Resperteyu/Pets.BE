using System.ComponentModel.DataAnnotations;

namespace Movrr.API.Authentication.Service.Models
{
  public class ResetPasswordRequest
  {
    [Required]
    public string Token { get; set; }

    [Required(ErrorMessage = "Required")]
    [MinLength(6, ErrorMessage = "Minimum 6 characters")]
    [Display(Name = "New Pasword")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Required")]
    [Compare("Password", ErrorMessage = "Passwords don't match")]
    [Display(Name = "Confirm Password")]
    public string ConfirmPassword { get; set; }
  }
}