using System.ComponentModel.DataAnnotations;

namespace Movrr.API.Authentication.Service.Models
{
  public class ValidateResetTokenRequest
  {
    [Required]
    public string Token { get; set; }
  }
}