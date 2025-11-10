using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class PatientProfileRepository : IPatientProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public PatientProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task AddAsync(PatientProfile profile)
        {
            await _context.Patients.AddAsync(profile);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(PatientProfile profile)
        {
            _context.Patients.Remove(profile);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<PatientProfile>> GetAllAsync()
        {
            return await _context.Patients.ToListAsync();
        }
        public async Task<PatientProfile> GetByIdAsync(int id)
        {
            return await _context.Patients.FindAsync(id);
        }
        public async Task<PatientProfile> GetByUserIdAsync(string userId)
        {
            return await _context.Patients
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }
        public async Task UpdateAsync(PatientProfile profile)
        {
            _context.Patients.Update(profile);
            await _context.SaveChangesAsync();
        }
    }
}
