using Movrr.API.Authentication.Service.Models;
using System.Threading.Tasks;

namespace Movrr.API.Authentication.Service
{
  public interface IAccountService
  {
    AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
    AuthenticateResponse RefreshToken(string token, string ipAddress);
    void RevokeToken(string token, string ipAddress);
    Task<AuthenticateResponse> RegisterAsync(RegisterRequest model, string ipAddress);
    void VerifyEmail(string token);
    Task ForgotPasswordAsync(ForgotPasswordRequest model);
    void ValidateResetToken(ValidateResetTokenRequest model);
    void ResetPassword(ResetPasswordRequest model);
    AccountResponse GetById(int id);
    AccountResponse Update(int id, UpdateRequest model);
    void Delete(int id);
  }
}