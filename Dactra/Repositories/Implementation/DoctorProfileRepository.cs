using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class DoctorProfileRepository : IDoctorProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(DoctorProfile profile)
        {
            await _context.Doctors.AddAsync(profile);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(DoctorProfile profile)
        {
            _context.Doctors.Remove(profile);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<DoctorProfile>> GetAllAsync()
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .ToListAsync();
        }
        public async Task<DoctorProfile> GetByIdAsync(int id)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<DoctorProfile> GetByUserEmail(string email)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.User.Email == email);
        }

        public async Task<DoctorProfile> GetByUserIdAsync(string userId)
        {
            return await _context.Doctors
                .Include(d => d.User)
                .Include(d => d.specialization)
                .FirstOrDefaultAsync(d => d.UserId == userId);
        }
        public async Task UpdateAsync(DoctorProfile profile)
        {
            _context.Doctors.Update(profile);
            await _context.SaveChangesAsync();
        }
    }
}
