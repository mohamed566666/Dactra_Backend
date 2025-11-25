using Dactra.Models;
using System.Security.Claims;

namespace Dactra.Services.Interfaces
{
    public interface ITokenService
    {
       string CreateToken(ApplicationUser user);
        Task<string> CreateRefreshToken(ApplicationUser user);
        ClaimsPrincipal? ValidateAccessToken(string token);
        Task<(string? AccessToken, string? Message)> RefreshAccessTokenAsync(string refreshToken);
        Task<ApplicationUser?> GetUserByRefreshToken(string refreshToken);

    }
}
