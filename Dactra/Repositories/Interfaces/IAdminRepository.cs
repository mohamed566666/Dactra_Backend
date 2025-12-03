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
        Task DeleteQuestions(Questions questions);
        Task<Post>? GetPostById(string id);
         Task DeletePost(Post questions);

    }
}
