using System.ComponentModel.DataAnnotations;

namespace Movrr.API.Authentication.Service.Models
{
  public class VerifyEmailRequest
  {
    [Required]
    public string Token { get; set; }
  }
}
