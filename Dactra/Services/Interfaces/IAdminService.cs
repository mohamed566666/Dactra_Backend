using Dactra.DTOs.Admin;

namespace Dactra.Services.Interfaces
{
    public interface IAdminService
    {
        Task<string> AddAdmin(CreateAdminDto dto);
        Task<IList<ApplicationUser>> GetAdmins();
        Task<bool> DeleteAdmin(string id);
        Task<ApplicationUser> GetById(string id);
        Task<ApplicationUser> GetByEmail(string email);
        Task AddToAdminRole(ApplicationUser user);
        Task<bool> DeleteAppUser(string id);
        Task<bool> DeleteQuestions(string id);
        Task<bool> DeletePosts(string id);
        Task<StatsDto> GetSummary();
        Task<Dictionary<string, int>> GetWeeklyAppointmentsCount();
        Task<List<patientinfoDto>> patientinfo(int page, int pageSize);
        Task<List<postInfoDto>> postinfo(int page, int pageSize);
        Task<List<questionInfoDto>> questioninfo(int page, int pageSize);
        Task<List<DoctorAdminInfoDTO>> GetDoctorsAsync(int page, int pageSize);
        Task<List<MedicalProviderAdminDTO>> GetLabsAsync(int page, int pageSize);
        Task<List<MedicalProviderAdminDTO>> GetScansAsync(int page, int pageSize);
    }
}
