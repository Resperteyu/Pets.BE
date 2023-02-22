using System;
using System.Text.Json.Serialization;

namespace Movrr.API.Authentication.Service.Models
{
  public class AuthenticateResponse
  {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public DateTime Expires { get; set; }
    public DateTime? Updated { get; set; }
    public bool IsVerified { get; set; }
    public string JwtToken { get; set; }
    public string RefreshToken { get; set; }
  }
}