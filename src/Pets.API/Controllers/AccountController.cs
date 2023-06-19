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
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Pets.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly PetsDbContext _context;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly IEmailSender _emailSender;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager,
            IEmailSender emailSender,
            TokenValidationParameters tokenValidationParameters,
            PetsDbContext context,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _configuration = configuration;
            _tokenValidationParameters = tokenValidationParameters;
            _emailSender = emailSender;
            _context = context;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel request)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);

                if (existingUser == null || !await _signInManager.CanSignInAsync(existingUser))
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Invalid authentication request"
                        }
                    });
                }

                // Now we need to check if the user has inputed the right password
                var isCorrect = await _userManager.CheckPasswordAsync(existingUser, request.Password);

                if (isCorrect)
                {
                    var jwtToken = await GenerateJwtToken(existingUser);

                    return Ok(jwtToken);
                }
                else
                {
                    // We dont want to give to much information on why the request has failed for security reasons
                    return BadRequest(new RegistrationResponse()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Invalid authentication request"
                        }
                    });
                }
            }

            return BadRequest(new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>()
                {
                    "Invalid payload"
                }
            });
        }

        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel request)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByEmailAsync(request.Email);

                if (existingUser != null)
                {
                    return BadRequest(new RegistrationResponse()
                    {
                        Success = false,
                        Errors = new List<string>()
                        {
                            "Email already exist"
                        }
                    });
                }

                var newUser = new ApplicationUser() { Email = request.Email, UserName = request.Email };
                var isCreated = await _userManager.CreateAsync(newUser, request.Password);

                if (!isCreated.Succeeded)
                {
                    return new JsonResult(new RegistrationResponse()
                    {
                        Success = false,
                        Errors = isCreated.Errors.Select(x => x.Description).ToList()
                    });
                }

                var jwtToken = await GenerateJwtToken(newUser);

                var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
                var confirmationLink = $"https://localhost:5001/Verify/?email={newUser.Email}&token={token}";
                await _emailSender.SendEmailAsync(newUser.Email, "Confirmation email link", confirmationLink);

                return Ok(jwtToken);

            }

            return BadRequest(new RegistrationResponse()
            {
                Success = false,
                Errors = new List<string>(){
                    "Invalid payload"
                }
            });
        }

        [HttpPost]
        [Route("register-admin")]
        public async Task<IActionResult> RegisterAdmin([FromBody] RegisterModel request)
        {
            var userExists = await _userManager.FindByNameAsync(request.Username);
            if (userExists != null)
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = new List<string> { "User already exists!" } });

            ApplicationUser user = new()
            {
                Email = request.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = request.Username
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new Response { Status = "Error", Message = new List<string> { "User creation failed! Please check user details and try again." } });

            if (!await _roleManager.RoleExistsAsync(UserRoles.Admin))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.Admin));
            if (!await _roleManager.RoleExistsAsync(UserRoles.User))
                await _roleManager.CreateAsync(new IdentityRole(UserRoles.User));

            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Admin);
            }
            if (await _roleManager.RoleExistsAsync(UserRoles.Admin))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.User);
            }
            return Ok(new Response { Status = "Success", Message = new List<string> { "User created successfully!" } });
        }

        [HttpPost]
        [Route("forgot-password")]
        public async Task<IActionResult> ForgotPassword([Required] string email)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);


                    var confirmationLink = $"https://localhost:5001/ResetPassword/?email={user.Email}&token={token}";
                    await _emailSender.SendEmailAsync(user.Email, "Confirmation email link", confirmationLink);
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Route("reset-password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            // TODO Not use RegistrationResponse
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegistrationResponse()
                {
                    Success = false,
                    Errors = new List<string>(){
                    "Invalid payload"
                }
                });
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user is not null)
            {
                var resetPassResult = await _userManager.ResetPasswordAsync(user, request.Token, request.Password);
                if (!resetPassResult.Succeeded)
                { 
                    return new JsonResult(new RegistrationResponse()
                    {
                        Success = false,
                        Errors = resetPassResult.Errors.Select(x => x.Description).ToList()
                    });
                }
            }

            return NoContent();
        }

        [HttpPost]
        [Route("verify")]
        public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string token)
        {
            if (!ModelState.IsValid) // TODO Create model? Don't use RegistrationResponse
            {
                return BadRequest(new RegistrationResponse()
                {
                    Success = false,
                    Errors = new List<string>(){
                    "Invalid payload"
                }
                });
            }

            var user = await _userManager.FindByEmailAsync(email);
            if (user is not null)
            {
                var verifyResponse = await _userManager.ConfirmEmailAsync(user, token);
                if (!verifyResponse.Succeeded)
                {
                    return new JsonResult(new RegistrationResponse()
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
                    return BadRequest(new RegistrationResponse()
                    {
                        Errors = new List<string>() {
                    "Invalid tokens"
                },
                        Success = false
                    });
                }

                return Ok(res);
            }

            return BadRequest(new RegistrationResponse()
            {
                Errors = new List<string>() {
                "Invalid payload"
            },
                Success = false
            });
        }

        [Authorize]
        [HttpPost]
        [Route("revoke/{username}")]
        public async Task<IActionResult> Revoke(string username)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user == null) return BadRequest("Invalid user name");

            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);

            return NoContent();
        }

        [Authorize]
        [HttpPost]
        [Route("revoke-all")]
        public async Task<IActionResult> RevokeAll()
        {
            var users = _userManager.Users.ToList();
            foreach (var user in users)
            {
                user.RefreshToken = null;
                await _userManager.UpdateAsync(user);
            }

            return NoContent();
        }

        private async Task<AuthResult> VerifyToken(TokenModel tokenRequest)
        {
            try
            {
                _tokenValidationParameters.ValidateLifetime = false;
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;
                var principal = tokenHandler.ValidateToken(tokenRequest.Token, _tokenValidationParameters, out securityToken);

                var jwtSecurityToken = securityToken as JwtSecurityToken;
                if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                    throw new SecurityTokenException("Invalid token");

                var storedRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequest.RefreshToken);

                if (storedRefreshToken == null)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "refresh token doesnt exist" },
                        Success = false
                    };
                }

                if (DateTime.UtcNow > storedRefreshToken.ExpiryDate)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "Refresh Token has expired" },
                        Success = false
                    };
                }

                if (storedRefreshToken.IsUsed)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "Refresh Token has been used" },
                        Success = false
                    };
                }

                if (storedRefreshToken.IsRevoked)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "Refresh Token has been revoked" },
                        Success = false
                    };
                }

                var jti = principal?.Claims?.SingleOrDefault(x => x.Type == JwtRegisteredClaimNames.Jti)?.Value;

                if (storedRefreshToken.JwtId != jti)
                {
                    return new AuthResult()
                    {
                        Errors = new List<string>() { "The Refresh Token doesn't matech the saved token" },
                        Success = false
                    };
                }

                storedRefreshToken.IsUsed = true;
                _context.RefreshTokens.Update(storedRefreshToken);
                await _context.SaveChangesAsync();

                var dbUser = await _userManager.FindByIdAsync(storedRefreshToken.UserId);
                return await GenerateJwtToken(dbUser);
            }
            catch (Exception ex)
            {
                // Log
                return null;
            }
        }

        private async Task<AuthResult> GenerateJwtToken(ApplicationUser user)
        {
            var jwtTokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(_configuration["JWT:Secret"]);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("Id", user.Id),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Iss, _configuration["JWT:ValidIssuer"]),         
                    new Claim(JwtRegisteredClaimNames.Aud, _configuration["JWT:ValidAudience"]),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration["JWT:TokenValidityInMinutes"])),
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

            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();

            return new AuthResult()
            {
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
