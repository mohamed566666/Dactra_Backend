using Dactra.Enums;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class MedicalTestProviderProfileRepository : IMedicalTestProviderProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public MedicalTestProviderProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<MedicalTestProviderProfile>> GetAllAsync()
        {
            return await _context.MedicalTestProviders
                .Include(m => m.User)
                .ToListAsync();
        }
        public async Task<MedicalTestProviderProfile> GetByIdAsync(int id)
        {
            return await _context.MedicalTestProviders.FindAsync(id);
        }
        public async Task<MedicalTestProviderProfile> GetByUserIdAsync(string userId)
        {
            return await _context.MedicalTestProviders
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.UserId == userId);
        }
        public async Task AddAsync(MedicalTestProviderProfile profile)
        {
            await _context.MedicalTestProviders.AddAsync(profile);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(MedicalTestProviderProfile profile)
        {
            _context.MedicalTestProviders.Update(profile);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(MedicalTestProviderProfile profile)
        {
            _context.MedicalTestProviders.Remove(profile);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<MedicalTestProviderProfile>> GetApprovedProfilesAsync(MedicalTestProviderType? type = null)
        {
            var query =  _context.MedicalTestProviders
                .Where(m => m.IsApproved);
            if (type.HasValue)
            {
                query = query.Where(m => m.Type == type.Value);
            }
            return await query.OrderByDescending(M => M.Avg_Rating).ToListAsync();
        }

        public async Task<IEnumerable<MedicalTestProviderProfile>> GetProfilesByTypeAsync(MedicalTestProviderType type)
        {
            var profiles = await _context.MedicalTestProviders
                .Where(m => m.Type == type)
                .ToListAsync();
            return profiles;
        }
    }
}
