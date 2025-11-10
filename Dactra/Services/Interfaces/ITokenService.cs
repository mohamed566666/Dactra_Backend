using Dactra.Models;

namespace Dactra.Services.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
    }
}
