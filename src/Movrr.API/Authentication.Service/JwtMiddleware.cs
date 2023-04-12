using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PetDb;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Movrr.API.Authentication.Service
{
  public class JwtMiddleware
  {
    private readonly RequestDelegate _next;
    private readonly AuthSettings _authSettings;

    public JwtMiddleware(RequestDelegate next, IOptions<AuthSettings> authSettings)
    {
      _next = next;
      _authSettings = authSettings.Value;
    }

    public async Task Invoke(HttpContext context, PetDbContext dataContext)
    {
      var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

      if (token != null)
        await AttachAccountToContext(context, dataContext, token);

      await _next(context);
    }

    private async Task AttachAccountToContext(HttpContext context, PetDbContext dataContext, string token)
    {
      try
      {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(Environment.GetEnvironmentVariable("JWTSecret"));
        tokenHandler.ValidateToken(token, new TokenValidationParameters
        {
          ValidateIssuerSigningKey = true,
          IssuerSigningKey = new SymmetricSecurityKey(key),
          ValidateIssuer = false,
          ValidateAudience = false,
          // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
          ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);

        var jwtToken = (JwtSecurityToken)validatedToken;
        var accountId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

        // attach account to context on successful jwt validation
        context.Items["Account"] = await dataContext.Accounts.FindAsync(accountId);
      }
      catch
      {
        // do nothing if jwt validation fails
        // account is not attached to context so request won't have access to secure routes
      }
    }
  }
}