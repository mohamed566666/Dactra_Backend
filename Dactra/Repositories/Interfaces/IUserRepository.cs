using Dactra.Models;
using Microsoft.AspNetCore.Identity;

namespace Dactra.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        public Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<ApplicationUser?> GetUserByIdAsync(string id);
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        public Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task DeleteUserAsync(ApplicationUser user);
    }
}
