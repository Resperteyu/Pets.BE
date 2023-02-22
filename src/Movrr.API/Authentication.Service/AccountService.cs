using AutoMapper;
using BC = BCrypt.Net.BCrypt;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Movrr.API.Authentication.Service.Models;
using Movrr.API.Authentication.Service.Entities;
using Movrr.API.Email.Service;
using System.Threading.Tasks;
using Movrr.API.Helpers;

namespace Movrr.API.Authentication.Service
{
  public class AccountService : IAccountService
  {
    private readonly IMapper _mapper;
    private readonly AuthSettings _authSettings;
    private readonly IEmailService _emailService;
    private readonly DataContext _context;

    public AccountService(DataContext context, IMapper mapper, IOptions<AuthSettings> authSettings,
      IEmailService emailService)
    {
      _mapper = mapper;
      _authSettings = authSettings.Value;
      _emailService = emailService;
      _context = context;
    }

    public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
    {
      var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

      if (account == null || !BC.Verify(model.Password, account.PasswordHash))
        throw new AppException("Email or password is incorrect");

      var (jwtToken, expires) = GenerateJwtToken(account);
      var refreshToken = GenerateRefreshToken(ipAddress);
      account.RefreshTokens.Add(refreshToken);

      RemoveOldRefreshTokens(account);

      _context.Update(account);
      _context.SaveChanges();

      var response = _mapper.Map<AuthenticateResponse>(account);
      response.JwtToken = jwtToken;
      response.Expires = expires;
      response.RefreshToken = refreshToken.Token;
      return response;
    }

    public AuthenticateResponse RefreshToken(string token, string ipAddress)
    {
      var (refreshToken, account) = GetRefreshToken(token);

      // replace old refresh token with a new one and save
      var newRefreshToken = GenerateRefreshToken(ipAddress);
      refreshToken.Revoked = DateTime.UtcNow;
      refreshToken.RevokedByIp = ipAddress;
      refreshToken.ReplacedByToken = newRefreshToken.Token;
      account.RefreshTokens.Add(newRefreshToken);

      RemoveOldRefreshTokens(account);

      _context.Update(account);
      _context.SaveChanges();

      // generate new jwt
      var (jwtToken, expires) = GenerateJwtToken(account);

      var response = _mapper.Map<AuthenticateResponse>(account);
      response.JwtToken = jwtToken;
      response.Expires = expires;
      response.RefreshToken = newRefreshToken.Token;
      return response;
    }

    public void RevokeToken(string token, string ipAddress)
    {
      var (refreshToken, account) = GetRefreshToken(token);

      // revoke token and save
      refreshToken.Revoked = DateTime.UtcNow;
      refreshToken.RevokedByIp = ipAddress;
      _context.Update(account);
      _context.SaveChanges();
    }

    public async Task<AuthenticateResponse> RegisterAsync(RegisterRequest model, string ipAddress)
    {
      if (_context.Accounts.Any(x => x.Email == model.Email))
      {
        return null;
      }

      var account = _mapper.Map<Account>(model);

      account.Created = DateTime.UtcNow;
      account.VerificationToken = RandomTokenString();

      account.PasswordHash = BC.HashPassword(model.Password);

      var refreshToken = GenerateRefreshToken(ipAddress);
      account.RefreshTokens = new List<RefreshToken> { refreshToken };

      _context.Accounts.Add(account);
      _context.SaveChanges();

      await SendVerificationEmail(account);

      var response = _mapper.Map<AuthenticateResponse>(account);
      var (jwtToken, expires) = GenerateJwtToken(account);
      response.JwtToken = jwtToken;
      response.Expires = expires;
      response.RefreshToken = refreshToken.Token;

      return response;
    }

    public void VerifyEmail(string token)
    {
      var account = _context.Accounts.SingleOrDefault(x => x.VerificationToken == token);

      if (account == null) throw new AppException("Verification failed");

      account.Verified = DateTime.UtcNow;
      account.VerificationToken = null;

      _context.Accounts.Update(account);
      _context.SaveChanges();
    }

    public async Task ForgotPasswordAsync(ForgotPasswordRequest model)
    {
      var account = _context.Accounts.SingleOrDefault(x => x.Email == model.Email);

      if (account == null) return;

      account.ResetToken = RandomTokenString();
      account.ResetTokenExpires = DateTime.UtcNow.AddDays(1);

      _context.Accounts.Update(account);
      _context.SaveChanges();

      await SendPasswordResetEmail(account);
    }

    public void ValidateResetToken(ValidateResetTokenRequest model)
    {
      var account = _context.Accounts.SingleOrDefault(x =>
          x.ResetToken == model.Token &&
          x.ResetTokenExpires > DateTime.UtcNow);

      if (account == null)
        throw new AppException("Invalid token");
    }

    public void ResetPassword(ResetPasswordRequest model)
    {
      var account = _context.Accounts.SingleOrDefault(x =>
          x.ResetToken == model.Token &&
          x.ResetTokenExpires > DateTime.UtcNow);

      if (account == null)
        throw new AppException("Invalid token");

      account.PasswordHash = BC.HashPassword(model.Password);
      account.PasswordReset = DateTime.UtcNow;
      account.ResetToken = null;
      account.ResetTokenExpires = null;

      _context.Accounts.Update(account);
      _context.SaveChanges();
    }

    public AccountResponse GetById(int id)
    {
      var account = GetAccount(id);
      return _mapper.Map<AccountResponse>(account);
    }

    public AccountResponse Update(int id, UpdateRequest model)
    {
      var account = GetAccount(id);

      // validate
      if (account.Email != model.Email && _context.Accounts.Any(x => x.Email == model.Email))
        throw new AppException($"Email '{model.Email}' is already taken");

      // hash password if it was entered
      if (!string.IsNullOrEmpty(model.Password))
        account.PasswordHash = BC.HashPassword(model.Password);

      // copy model to account and save
      _mapper.Map(model, account);
      account.Updated = DateTime.UtcNow;
      _context.Accounts.Update(account);
      _context.SaveChanges();

      return _mapper.Map<AccountResponse>(account);
    }

    public void Delete(int id)
    {
      var account = GetAccount(id);
      _context.Accounts.Remove(account);
      _context.SaveChanges();
    }

    private Account GetAccount(int id)
    {
      var account = _context.Accounts.Find(id);
      if (account == null) throw new KeyNotFoundException("Account not found");
      return account;
    }

    private (RefreshToken, Account) GetRefreshToken(string token)
    {
      var account = _context.Accounts.SingleOrDefault(u => u.RefreshTokens.Any(t => t.Token == token));
      if (account == null) throw new AppException("Invalid token");
      var refreshToken = account.RefreshTokens.Single(x => x.Token == token);
      if (!refreshToken.IsActive) throw new AppException("Invalid token");
      return (refreshToken, account);
    }

    private (string, DateTime) GenerateJwtToken(Account account)
    {
      var tokenHandler = new JwtSecurityTokenHandler();
      var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWTSecret"));
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(new[] { new Claim("id", account.Id.ToString()) }),
        Expires = DateTime.UtcNow.AddMinutes(15),
        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
      };
      var token = tokenHandler.CreateToken(tokenDescriptor);
      var jwt = tokenHandler.WriteToken(token);
      var expires = tokenHandler.ReadJwtToken(jwt).ValidTo;

      return (jwt, expires);
    }

    private RefreshToken GenerateRefreshToken(string ipAddress)
    {
      return new RefreshToken
      {
        Token = RandomTokenString(),
        Expires = DateTime.UtcNow.AddDays(7),
        Created = DateTime.UtcNow,
        CreatedByIp = ipAddress
      };
    }

    private void RemoveOldRefreshTokens(Account account)
    {
      account.RefreshTokens.RemoveAll(x =>
          !x.IsActive &&
          x.Created.AddDays(_authSettings.RefreshTokenTTL) <= DateTime.UtcNow);
    }

    private string RandomTokenString()
    {
      using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
      var randomBytes = new byte[40];
      rngCryptoServiceProvider.GetBytes(randomBytes);
      // convert random bytes to hex string
      return BitConverter.ToString(randomBytes).Replace("-", "");
    }

    private async Task SendVerificationEmail(Account account)
    {
      var verifyUrl = $"https://movrr.azurewebsites.net//password/verify?token={account.VerificationToken}";
      var content = $@"<p>Please click the below link to verify your email address:</p>
                             <p><a href=""{verifyUrl}"">{verifyUrl}</a></p>";

      var html = $@"<h4>Verify Email</h4>
                      <p>Thanks for registering!</p>
                      {content}";

      var message = new Message(new string[] { account.Email }, "Movrr - Verify Email", html, null);
      await _emailService.SendEmailAsync(message, null, null);
    }

    private async Task SendPasswordResetEmail(Account account)
    {
    
        var resetUrl = $"https://movrr.azurewebsites.net//password/reset?token={account.ResetToken}";
        var content = $@"<p>Please click the below link to reset your password, the link will be valid for 1 day:</p>
                             <p><a href=""{resetUrl}"">{resetUrl}</a></p>";
      var html = $@"<h4>Reset Password Email</h4>
                      {content}";

      var message = new Message(new string[] { account.Email }, "Movrr - Reset Password", html, null);
      await _emailService.SendEmailAsync(message, null, null);
    }
  }
}