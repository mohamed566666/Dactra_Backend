using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace Dactra.Repositories.Implementation
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public AdminRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager
                               )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        }
        public async Task AddToAdminRole(ApplicationUser user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null");

            if (_roleManager == null)
                throw new Exception("RoleManager is not initialized");

        

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
                await _userManager.AddToRoleAsync(user, "Admin");
        }

    
        public async Task<IdentityResult> CreateUser(ApplicationUser user, string password)
        {
            return await _userManager.CreateAsync(user, password);

        }

        public async Task DeleteUser(ApplicationUser user)
        {
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.DeleteAsync(user);
        }

        public async Task<IList<ApplicationUser>> GetAdmins()
        {
            return await _userManager.GetUsersInRoleAsync("Admin");
        }

        public Task<ApplicationUser> GetByEmail(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public async Task<ApplicationUser> GetById(string id)
        {
           return await _userManager.FindByIdAsync(id);
        }

        public async Task<bool> IsAdmin(ApplicationUser user)
        {
           return await _userManager.IsInRoleAsync(user, "Admin");
        }
    }
}
