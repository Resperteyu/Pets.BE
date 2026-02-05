using System.Security.Claims;
using System.Threading.Tasks;
using Pets.API.Responses;
using Pets.Db.Models;

namespace Pets.API.Services
{
    public interface ITokenService
    {
        Task<AuthResponse> GenerateAuthResponse(ApplicationUser user);
        ClaimsPrincipal GetPrincipalFromToken(string token);
    }
} 