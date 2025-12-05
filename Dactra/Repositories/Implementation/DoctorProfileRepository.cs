using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class DoctorProfileRepository : GenericRepository<DoctorProfile> , IDoctorProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorProfileRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public override async Task<IEnumerable<DoctorProfile>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .ToListAsync();
        }
        public override async Task<DoctorProfile?> GetByIdAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DoctorProfile?> GetByUserEmail(string email)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.User.Email == email);
        }

        public async Task<DoctorProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }
    }
}
