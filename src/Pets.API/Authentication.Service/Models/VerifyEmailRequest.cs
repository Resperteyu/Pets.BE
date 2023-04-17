using System.ComponentModel.DataAnnotations;

namespace Pets.API.Authentication.Service.Models
{
  public class VerifyEmailRequest
  {
    [Required]
    public string Token { get; set; }
  }
}
