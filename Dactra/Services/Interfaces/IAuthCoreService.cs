using Dactra.Models;

namespace Dactra.Services.Interfaces
{
        public interface IAuthCoreService{


            Task<(string AccessToken, string RefreshToken, string? Error)> ExternalLoginAsync(
                ApplicationUser user,
                string? deviceInfo = null,
                string? ipAddress = null

           );

        }
}
