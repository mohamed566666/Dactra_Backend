using Dactra.DTOs.Admin;

namespace Dactra.Repositories.Interfaces
{
    public interface IAdminRepository
    {
        Task<ApplicationUser> GetById(string id);
        Task<ApplicationUser> GetByEmail(string email);
        Task<IList<ApplicationUser>> GetAdmins();
        Task<IdentityResult> CreateUser(ApplicationUser user, string password);
        Task AddToAdminRole(ApplicationUser user);
        Task<bool> IsAdmin(ApplicationUser user);
        Task DeleteUser(ApplicationUser user);
        Task <Questions>? GetQuestionsById (string id);    
        Task DeleteQuestions(Questions questions );
        Task<Post>? GetPostById(string id);
         Task DeletePost(Post questions);
        Task<int> GetDoctorsCount();
        Task<int> GetPatientsCount();
        Task<int> GetLabCount();
        Task<int> GetScanCount();
        Task<Dictionary<string, int>> GetWeeklyAppointmentsCount();

        Task<List< patientinfoDto>> patientinfo(int page , int pageSize );
        Task<List< questionInfoDto>> questioninfo(int page  , int pageSize );
        Task<List< postInfoDto>> postinfo(int page , int pageSize );
        Task<List<DoctorAdminInfoDTO>> GetAllDoctorsAsync(int page, int pageSize);
        Task<List<MedicalProviderAdminDTO>> GetAllLabsAsync(int page, int pageSize);
        Task<List<MedicalProviderAdminDTO>> GetAllScansAsync(int page, int pageSize);
    }
}
