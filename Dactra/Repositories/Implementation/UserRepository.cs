using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MimeKit.Encodings;

namespace Dactra.Repositories.Implementation
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserRepository(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
            => await _userManager.CreateAsync(user, password);
        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email)!;
        }
        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
            => await _userManager.UpdateAsync(user);
        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }
        
        public async Task DeleteUserAsync(ApplicationUser user)
            => await _userManager.DeleteAsync(user);

        public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
            => await _userManager.Users.AsNoTracking().ToListAsync();
    }
}
