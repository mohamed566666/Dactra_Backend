using Dactra.DTOs.Admin;

namespace Dactra.Repositories.Implementation
{
    public class AdminRepository : IAdminRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminRepository(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager, ApplicationDbContext context
                               )
        {
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _context = context;
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

        public async Task DeletePost(Post post)
        {
            
                post.isDeleted = !post.isDeleted;
                await _context.SaveChangesAsync();
            
           
           
        }

        public async Task DeleteQuestions(Questions questions)
        {
            
            
                questions.isDeleted = !questions.isDeleted;
                _context.Questions.Update(questions);
                await _context.SaveChangesAsync();
            
            
        }

        public async Task DeleteUser(ApplicationUser user)
        {
          
                user.isDeleted = !user.isDeleted;
                await _userManager.UpdateAsync(user);
                await _context.SaveChangesAsync();
            
         
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

        public async Task<int> GetDoctorsCount()
        {
            var doctorRoleId = await _context.Roles
                .Where(r => r.Name == "Doctor")
                .Select(r => r.Id)
                .FirstOrDefaultAsync();

            var doctorsCount = await _context.UserRoles
                .Where(ur => ur.RoleId == doctorRoleId)
                .Select(ur => ur.UserId)
                .Distinct()
                .CountAsync();
            return doctorsCount;
        }

        public async Task<int> GetPatientsCount()
        {
            var patientRoleId = await _context.Roles
              .Where(r => r.Name == "Patient")
              .Select(r => r.Id)
              .FirstOrDefaultAsync();

            var patientCount = await _context.UserRoles
                .Where(ur => ur.RoleId == patientRoleId)
                .Select(ur => ur.UserId)
                .Distinct()
                .CountAsync();
            return patientCount;

        }

        public Task<Post>? GetPostById(string id)
        {
           return _context.Posts.FirstOrDefaultAsync(s=>s.Id==int.Parse(id));   
        }

        public async Task<int> GetLabCount()
        {
            return await _context.MedicalTestProviders.Where(p => p.Type==Enums.MedicalTestProviderType.Lab).CountAsync();
        }

        public async Task<Questions>? GetQuestionsById(string id)
        {
           return  await _context.Questions.FirstOrDefaultAsync(s=>s.Id==int.Parse(id));
        }

        public async Task<int> GetScanCount()
        {
            return await _context.MedicalTestProviders.Where(p => p.Type == Enums.MedicalTestProviderType.Scan).CountAsync();
        }

        public async Task<Dictionary<string, int>> GetWeeklyAppointmentsCount()
        {
            var today = DateTime.UtcNow.Date;

            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + 6); 
            var endOfWeek = startOfWeek.AddDays(6);

            var appointments = await _context.PatientAppointments
                .Where(a => a.BookedAt.Date >= startOfWeek && a.BookedAt.Date <= endOfWeek)
                .ToListAsync();

            var countsByDay = appointments
                .GroupBy(a => a.BookedAt.DayOfWeek)
                .ToDictionary(
                    g => CultureInfo.CurrentCulture.DateTimeFormat.GetDayName(g.Key),
                    g => g.Count()
                );

           
            var orderedDays = new[] { "Saturday", "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday" };
            var result = orderedDays.ToDictionary(day => day, day => countsByDay.ContainsKey(day) ? countsByDay[day] : 0);
            return result;
        }

        public async Task<bool> IsAdmin(ApplicationUser user)
        {
           return await _userManager.IsInRoleAsync(user, "Admin");
        }

        public async Task<List<patientinfoDto>> patientinfo(int page = 1, int pageSize = 10)
        {
            var result = await _context.Patients
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new patientinfoDto
                {
                    id = p.UserId,
                    fullName = p.FirstName + " " + p.LastName,
                    Email = p.User.Email,
                    isDeleted = p.User.isDeleted
                })
                .ToListAsync();

            return result;
        }


        public async Task<List<postInfoDto>> postinfo(int page = 1, int pageSize = 10)
        {
            var result = await _context.Posts
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new postInfoDto
                {
                    id = p.Id.ToString(),
                    title = p.title,
                    FullName = p.Doctor.FirstName + " " + p.Doctor.LastName,
                    createdAt = p.CreatedAt,
                    isDeleted = p.isDeleted
                })
                .ToListAsync();

            return result;
        }


        public async Task<List<questionInfoDto>> questioninfo(int page = 1, int pageSize = 10)
        {
            var result = await _context.Questions
                .OrderBy(q => q.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(q => new questionInfoDto
                {
                    id = q.Id.ToString(),
                    Content = q.Text,
                    Pname = q.Patient.FirstName + " " + q.Patient.LastName,
                    createdAt = q.CreatedAt,
                    isDeleted = q.isDeleted
                })
                .ToListAsync();

            return result;
        }

        public async Task<List<DoctorAdminInfoDTO>> GetAllDoctorsAsync(int page, int pageSize)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .OrderBy(d => d.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(d => new DoctorAdminInfoDTO
                {
                    ProfileId = d.Id,
                    Name = d.FirstName + " " + d.LastName,
                    Email = d.User.Email,
                    IsApproved = d.IsApproved
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<MedicalProviderAdminDTO>> GetAllLabsAsync(int page, int pageSize)
        {
            return await _context.MedicalTestProviders
                .Include(p => p.User)
                .Where(p => p.Type == MedicalTestProviderType.Lab)
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new MedicalProviderAdminDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    PhoneNumber = p.User.PhoneNumber,
                    IsApproved = p.IsApproved,
                    Address = p.Address
                })
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<List<MedicalProviderAdminDTO>> GetAllScansAsync(int page, int pageSize)
        {
            return await _context.MedicalTestProviders
                .Include(p => p.User)
                .Where(p => p.Type == MedicalTestProviderType.Scan)
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new MedicalProviderAdminDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    PhoneNumber = p.User.PhoneNumber,
                    IsApproved = p.IsApproved,
                    Address = p.Address
                })
                .AsNoTracking()
                .ToListAsync();
        }

    }
}
