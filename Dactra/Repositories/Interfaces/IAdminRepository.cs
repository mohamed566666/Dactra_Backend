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
        Task <Question>? GetQuestionsById (string id);    
        Task DeleteQuestions(Question questions );
        Task<Post>? GetPostById(string id);
         Task DeletePost(Post questions);
        Task<int> GetDoctorsCount();
        Task<int> GetPatientsCount();
        Task<int> GetLabCount();
        Task<int> GetScanCount();
        Task<WeeklyAppointmentsResponse> GetWeeklyAppointmentsCount();

        Task<List<patientinfoDto>> patientinfo(int page, int pageSize , string? searchName = null);
        Task<List< questionInfoDto>> questioninfo(int page  , int pageSize );
        Task<List< postInfoDto>> postinfo(int page , int pageSize );
        Task<List<DoctorAdminInfoDTO>> GetAllDoctorsAsync(int page, int pageSize , string? searchName = null , ApprovalStatus? approvalStatus = null);
        Task<List<MedicalProviderAdminDTO>> GetAllLabsAsync(int page, int pageSize , string? searchName = null , ApprovalStatus? approvalStatus = null);
        Task<List<MedicalProviderAdminDTO>> GetAllScansAsync(int page, int pageSize , string? searchName = null , ApprovalStatus? approvalStatus = null);
    }
}
