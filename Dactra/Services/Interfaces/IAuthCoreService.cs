using Dactra.Models;

namespace Dactra.Services.Interfaces
{
    public interface IAuthCoreService
    {
<<<<<<< HEAD
      
=======
        public interface IAuthCoreService
        {
>>>>>>> b8b6a82 (repase)
            Task<(string AccessToken, string RefreshToken, string? Error)> ExternalLoginAsync(
                ApplicationUser user,
                string? deviceInfo = null,
                string? ipAddress = null
<<<<<<< HEAD
           );
=======
            );
        }
>>>>>>> b8b6a82 (repase)
    }
}
