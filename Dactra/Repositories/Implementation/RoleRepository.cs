namespace Dactra.Repositories.Implementation
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public RoleRepository(RoleManager<ApplicationRole> roleManager, UserManager<ApplicationUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }
        public async Task<List<string>> GetRolesNameAsync()
                 => await _roleManager.Roles.Select(r => r.Name).ToListAsync();
        public async Task<bool> RoleExistsAsync(string roleName)
            => await _roleManager.RoleExistsAsync(roleName);
        public async Task<IdentityResult> CreateRoleAsync(ApplicationRole role)
                    => await _roleManager.CreateAsync(role);
        public async Task<IdentityResult> AddUserToRoleAsync(ApplicationUser user, string roleName)
                    => await _userManager.AddToRoleAsync(user, roleName);
        public async Task<IList<string>> GetUserRolesAsync(ApplicationUser user)
            => await _userManager.GetRolesAsync(user);
        public async Task<IList<ApplicationRole>> GetAllRolesAsync()
            => await _roleManager.Roles.ToListAsync();
    }
}
