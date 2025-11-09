using Dactra.Models;

namespace Dactra.Interface
{
    public interface ITokenService
    {
        string CreateToken(ApplicationUser user);
    }
}
