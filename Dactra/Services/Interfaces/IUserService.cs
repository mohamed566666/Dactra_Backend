using Dactra.DTOs;
using Microsoft.AspNetCore.Identity;

namespace Dactra.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityResult> SendDTOforVerficatio(SendOTPtoMailDTO model);
        public Task<IdentityResult> RegisterAsync(RegisterDto model);
    }
}
