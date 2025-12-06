using Dactra.DTOs.Admin;
using Dactra.Models;
using Microsoft.AspNetCore.Identity;

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

        Task<List< patientinfoDto>> patientinfo();
        Task<List< questionInfoDto>> questioninfo();
        Task<List< postInfoDto>> postinfo();
    }
}
