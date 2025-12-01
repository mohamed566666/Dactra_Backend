using Dactra.Models;
using Dactra.DTOs.Admin;
namespace Dactra.Services.Interfaces
{
    public interface IAdminService
    {
        Task<string> SeedAdmin();
        Task<string> AddAdmin(CreateAdminDto dto);
        Task<IList<ApplicationUser>> GetAdmins();
        Task<string> DeleteAdmin(string id);
        Task<ApplicationUser> GetById(string id);
        Task<ApplicationUser> GetByEmail(string email);
        Task AddToAdminRole(ApplicationUser user);
    }
}
