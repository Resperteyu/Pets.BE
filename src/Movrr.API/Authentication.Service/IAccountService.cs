using Movrr.API.Authentication.Service.Models;
using System.Threading.Tasks;

namespace Movrr.API.Authentication.Service
{
  public interface IAccountService
  {
    Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, string ipAddress);
    Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress);
    Task RevokeTokenAsync(string token, string ipAddress);
    Task<AuthenticateResponse> RegisterAsync(RegisterRequest model, string ipAddress);
    Task VerifyEmailAsync(string token);
    Task ForgotPasswordAsync(ForgotPasswordRequest model);
    Task ValidateResetTokenAsync(ValidateResetTokenRequest model);
    Task ResetPasswordAsync(ResetPasswordRequest model);
    Task<AccountResponse> GetByIdAsync(int id);
    Task<AccountResponse> UpdateAsync(int id, UpdateRequest model);
    Task DeleteAsync(int id);
  }
}