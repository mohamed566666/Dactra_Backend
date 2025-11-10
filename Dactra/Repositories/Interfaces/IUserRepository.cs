using Dactra.Models;
using Microsoft.AspNetCore.Identity;

namespace Dactra.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        public Task<ApplicationUser> GetUserByEmailAsync(string email);
        public Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task DeleteUserAsync(ApplicationUser user);
    }
}
