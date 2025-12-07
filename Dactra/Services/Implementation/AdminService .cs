using Dactra.DTOs.Admin;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Diagnostics;

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

        public async Task<bool> DeleteAdmin(string id)
        {
            var user = await _repo.GetById(id);
            if (user == null) return false;

            var isAdmin = await _repo.IsAdmin(user);
            if (!isAdmin) return false;
            
            
                await _repo.DeleteUser(user);
                return true;
            
         
        }
        public async Task<bool> DeleteAppUser(string id)
        {
            var user = await _repo.GetById(id);
            if (user == null) return false;
            
            
                await _repo.DeleteUser(user);
                return true;
            
        
        }

        public async Task<IList<ApplicationUser>> GetAdmins()
        {
           return  await _repo.GetAdmins();
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

        public async Task<bool> DeleteQuestions(string id)
        {
            var questions =await  _repo.GetQuestionsById(id);
            if ( questions ==null)
            {
                return false;
            }
            await _repo.DeleteQuestions(questions);
            
            return true;
        }

        public async Task<bool> DeletePosts(string id)
        {
            var post = await  _repo.GetPostById(id);
            if (post == null)
            {
                return false ;
            }
            await _repo.DeletePost(post);
            return true;
        }

        public async Task<StatsDto> GetSummary()
        {
            return new StatsDto
            {
                DoctorsCount = await _repo.GetDoctorsCount(),
                PatientsCount = await _repo.GetPatientsCount(),
                LabCount = await _repo.GetLabCount(),
                ScanCount = await _repo.GetScanCount()
            };
        }

        public async Task<Dictionary<string, int>> GetWeeklyAppointmentsCount()
        {
            return await _repo.GetWeeklyAppointmentsCount();
        }

        public Task<List<patientinfoDto>> patientinfo(int page, int pageSize)
          => _repo.patientinfo(page, pageSize);

        public Task<List<postInfoDto>> postinfo(int page, int pageSize)
            => _repo.postinfo(page, pageSize);

        public Task<List<questionInfoDto>> questioninfo(int page, int pageSize)
            => _repo.questioninfo(page, pageSize);
    }
}
