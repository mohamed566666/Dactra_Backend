using Dactra.DTOs.Admin;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
namespace Dactra.Services.Implementation
{
    public class AdminService : IAdminService

    {
        private readonly IAdminRepository _repo;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminService(IAdminRepository repo, UserManager<ApplicationUser> userManager)
        {
            _repo = repo;
            _userManager = userManager;
        }

        public async Task<string> AddAdmin(CreateAdminDto dto)
        {
            var exists = await _repo.GetByEmail(dto.Email);
            if (exists != null)
                return "User already exists";

            var user = new ApplicationUser
            {
                Email = dto.Email,
                UserName = dto.Email
            };

            var result = await _repo.CreateUser(user, dto.Password);

            if (!result.Succeeded)
                return string.Join(" | ", result.Errors.Select(e => e.Description));

            await _repo.AddToAdminRole(user);
            return "Admin created successfully";
        }

        public async Task<string> DeleteAdmin(string id)
        {
            var user = await _repo.GetById(id);
            if (user == null) return "Admin not found";

            var isAdmin = await _repo.IsAdmin(user);
            if (!isAdmin) return "User is not an admin";

            await _repo.DeleteUser(user);
            return "Admin deleted";
        }

        public async Task<IList<ApplicationUser>> GetAdmins()
        {
           return  await _repo.GetAdmins();
        }

        public async Task<string> SeedAdmin()
        {
            var existing = await _repo.GetByEmail("rashadmostafa84@gmail.com");
            if (existing != null)
                return "Admin already exists";

            var newAdmin = new ApplicationUser
            {
                Email = "rashadmostafa84@gmail.com",
                UserName = "rashadmostafa84@gmail.com"
            };

            var result = await _repo.CreateUser(newAdmin, "12345678#Rr");

            if (!result.Succeeded)
                return "Failed: " + string.Join(" | ", result.Errors.Select(e => e.Description));

            await _repo.AddToAdminRole(newAdmin);
            return "Admin Seeded";
        
        }
        public async Task<ApplicationUser> GetById(string id)
        {
            var user = await _repo.GetById(id);
            if (user == null) return null;

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin) return null; 
            return user;
        }
        public async Task<ApplicationUser> GetByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null) return null;

            bool isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
            if (!isAdmin) return null; 
            return user;
        }
        public async Task AddToAdminRole(ApplicationUser user)
        {
          
            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                await _userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}
