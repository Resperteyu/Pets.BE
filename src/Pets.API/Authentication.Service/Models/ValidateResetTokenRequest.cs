using System.ComponentModel.DataAnnotations;

namespace Pets.API.Authentication.Service.Models
{
  public class ValidateResetTokenRequest
  {
    [Required]
    public string Token { get; set; }
  }
}