using Dactra.DTOs.AuthemticationDTOs;
using Dactra.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Dactra.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityResult> SendDTOforVerficatio(SendOTPtoMailDTO model);
        public Task<IdentityResult> RegisterAsync(RegisterDto model);
        public Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task UpdateAsync(ApplicationUser user);
        Task <IResult> LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal);

    }
}
