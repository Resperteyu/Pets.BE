using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Pets.API.Authentication.Service;
using Pets.API.Authentication.Service.Models;
using System;
using System.Threading.Tasks;

namespace Pets.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : BaseController
    {
        private readonly IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("authenticate")]
        public async Task<ActionResult<AuthenticateResponse>> Authenticate(AuthenticateRequest model)
        {
            var response = await _accountService.AuthenticateAsync(model, IpAddress());
            return Ok(response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<AuthenticateResponse>> RefreshToken(string refreshToken)
        {
            var response = await _accountService.RefreshTokenAsync(refreshToken, IpAddress());

            return Ok(response);
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(RevokeTokenRequest model)
        {
            var token = model.Token ?? model.RefreshToken;

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            if (!Account.OwnsToken(token))
                return Unauthorized(new { message = "Unauthorized" });

            await _accountService.RevokeTokenAsync(token, IpAddress());
            return Ok(new { message = "Token revoked" });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            var authenticateResponse = await _accountService.RegisterAsync(model, IpAddress());
            if (authenticateResponse == null)
                return StatusCode(409, "Email is already registered");
            return Ok(authenticateResponse);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            await _accountService.ForgotPasswordAsync(model);
            return Ok(new { message = "Please check your email for instructions" });
        }

        [HttpPost("validate-reset-token")]
        public async Task<IActionResult> ValidateResetToken(ValidateResetTokenRequest model)
        {
            await _accountService.ValidateResetTokenAsync(model);
            return Ok(new { message = "Token is valid" });
        }

        [Authorize]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AccountResponse>> GetById(Guid id)
        {
            if (id != Account.Id)
                return Unauthorized(new { message = "Unauthorized" });

            var account = await _accountService.GetByIdAsync(id);
            return Ok(account);
        }

        [Authorize]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<AccountResponse>> Update(Guid id, UpdateRequest model)
        {
            if (id != Account.Id)
                return Unauthorized(new { message = "Unauthorized" });

            var account = await _accountService.UpdateAsync(id, model);
            return Ok(account);
        }

        [Authorize]
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (id != Account.Id)
                return Unauthorized(new { message = "Unauthorized" });

            await _accountService.DeleteAsync(id);
            return Ok(new { message = "Account deleted successfully" });
        }

        private string IpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}
