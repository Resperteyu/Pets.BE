namespace Pets.API.Authentication.Service.Models
{
  public class RevokeTokenRequest
  {
    public string Token { get; set; }
    public string RefreshToken { get; set; }
  }
}