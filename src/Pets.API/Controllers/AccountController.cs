using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Pets.Db.Models;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using Pets.API.Requests;
using Pets.API.Responses;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using PetDb.Models;
using Pets.Db;
using Microsoft.EntityFrameworkCore;
using Pets.API.Services;

namespace Pets.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        EmailService emailService,
        TokenValidationParameters tokenValidationParameters,
        PetsDbContext context,
        IConfiguration configuration)
        : ControllerBase
    {
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthenticateResponse()
                {
                    Success = false,
                    Errors = ["Invalid payload"]
                });
            var existingUser = await userManager.FindByEmailAsync(request.Email);

            if (existingUser == null || !await signInManager.CanSignInAsync(existingUser))
            {
                return BadRequest(new AuthenticateResponse()
                {
                    Success = false,
                    Errors = ["Invalid authentication request"]
                });
            }

            // Now we need to check if the user has inputed the right password
            var isCorrect = await userManager.CheckPasswordAsync(existingUser, request.Password);

            if (isCorrect)
            {
                var authResponse = await GenerateAuthenticateResponse(existingUser);

                return Ok(authResponse);
            }
            else
            {
                // We dont want to give to much information on why the request has failed for security reasons
                return BadRequest(new AuthenticateResponse()
                {
                    Success = false,
                    Errors = ["Invalid authentication request"]
                });
            }

        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new AuthenticateResponse { Success = false, Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage)).ToList() });

            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null) 
                return BadRequest( new AuthenticateResponse{ Success = false, Errors = ["Email already exists"] });
            
            var role = string.IsNullOrWhiteSpace(request.Role) ? "PetOwner" : request.Role;
            
            var roleExists = await roleManager.RoleExistsAsync(role);
            if (!roleExists)
                return BadRequest(new AuthenticateResponse { Success = false, Errors = ["Role does not exist"] });

            var newUser = new ApplicationUser { Email = request.Email, UserName = request.Username };
            var createUserResult = await userManager.CreateAsync(newUser, request.Password);
            if (!createUserResult.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new AuthenticateResponse { Success = false, Errors = createUserResult.Errors.Select(x => x.Description).ToList() });

            var roleResult = await userManager.AddToRoleAsync(newUser, role);
            if (!roleResult.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new AuthenticateResponse { Success = false, Errors = roleResult.Errors.Select(x => x.Description).ToList() });

            var authResponse = await GenerateAuthenticateResponse(newUser);

            var token = await userManager.GenerateEmailConfirmationTokenAsync(newUser);
            await emailService.SendConfirmationEmailAsync(newUser.Email, token);

            return Ok(authResponse);
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NoContent();
            }

            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await emailService.SendForgotPasswordEmailAsync(user.Email, token);

            return NoContent();
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return NotFound();
            }

            var resetPassResult = await userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (!resetPassResult.Succeeded)
            {
                return BadRequest(new { Error = resetPassResult.ToString() });
            }

            return NoContent();
        }

        [HttpPost]
        [Route("verify")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
        {
            if (!ModelState.IsValid) // TODO Create model? Don't use RegistrationResponse
            {
                return BadRequest(new AuthenticateResponse()
                {
                    Success = false,
                    Errors = ["Invalid payload"]
                });
            }

            var user = await userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                var verifyResponse = await userManager.ConfirmEmailAsync(user, token);
                if (!verifyResponse.Succeeded)
                {
                    return new JsonResult(new AuthenticateResponse()
                    {
                        Success = false,
                        Errors = verifyResponse.Errors.Select(x => x.Description).ToList()
                    });
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Route("refresh-token")]
        public async Task<IActionResult> RefreshToken(TokenModel request)
        {
            if (ModelState.IsValid)
            {
                var res = await VerifyToken(request);

                if (res == null)
                {
                    return BadRequest(new AuthenticateResponse()
                    {
                        Errors = ["Invalid tokens"],
                        Success = false
                    });
                }

                return Ok(res);
            }

            return BadRequest(new AuthenticateResponse()
            {
                Errors = ["Invalid payload"],
                Success = false
            });
        }

        [Authorize]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null) return BadRequest("Invalid user name");

            user.RefreshToken = null;
            await userManager.UpdateAsync(user);

            return NoContent();
        }

        [Authorize]
        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            var users = userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await userManager.UpdateAsync(user);
            }

            return NoContent();
        }

        private async Task<AuthenticateResponse> VerifyToken(TokenModel tokenRequest)
        {
            try
            {
                tokenValidationParameters.ValidateLifetime = false;
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(tokenRequest.Token, tokenValidationParameters, out securityToken);

                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                var storedRefreshToken = await context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedRefreshToken == null)
                {
                    return new AuthenticateResponse()
                    {
                        Errors = ["refresh token doesnt exist"],
                        Success = false
                    };
                }

                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new AuthenticateResponse()
                    {
                        Errors = ["Refresh Token has expired"],
                        Success = false
                    };
                }

                if (storedRefreshToken.IsUsed)
                {
                    return new AuthenticateResponse()
                    {
                        Errors = ["Refresh Token has been used"],
                        Success = false
                    };
                }

                if (storedRefreshToken.IsRevoked)
                {
                    return new AuthenticateResponse()
                    {
                        Errors = ["Refresh Token has been revoked"],
                        Success = false
                    };
                }

                var jti = principal?.Claims?.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (storedRefreshToken.JwtId != jti)
                {
                    return new AuthenticateResponse()
                    {
                        Errors = ["The Refresh Token doesn't matech the saved token"],
                        Success = false
                    };
                }

                storedRefreshToken.IsUsed = true;
                context.RefreshTokens.Update(storedRefreshToken);
                await context.SaveChangesAsync();

                var dbUser = await userManager.FindByIdAsync(storedRefreshToken.UserId.ToString());
                return await GenerateAuthenticateResponse(dbUser);
            }
            catch (Exception ex)
            {
                // Log
                return null;
            }
        }

        private async Task<AuthenticateResponse> GenerateAuthenticateResponse(ApplicationUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(configuration["JWT:Secret"]);

            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                    new Claim("Id", user.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Iss, configuration["JWT:ValidIssuer"]),
                    new Claim(JwtRegisteredClaimNames.Aud, configuration["JWT:ValidAudience"]),
                    new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("emailVerified", user.EmailConfirmed.ToString()),
                    new Claim("phoneVerified", user.PhoneNumberConfirmed.ToString()),
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(configuration["JWT:TokenValidityInMinutes"])),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);

            var refreshToken = new RefreshToken()
            {
                JwtId = token.Id,
                IsUsed = false,
                UserId = user.Id,
                AddedDate = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddYears(1),
                IsRevoked = false,
                Token = RandomString(25) + Guid.NewGuid()
            };

            await context.RefreshTokens.AddAsync(refreshToken);
            await context.SaveChangesAsync();

            return new AuthenticateResponse()
            {
                Id = user.Id,
                Token = jwtToken,
                Success = true,
                RefreshToken = refreshToken.Token
            };
        }

        private string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
