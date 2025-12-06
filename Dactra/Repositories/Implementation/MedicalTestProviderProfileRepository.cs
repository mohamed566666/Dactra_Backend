using Dactra.Enums;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class MedicalTestProviderProfileRepository : GenericRepository<MedicalTestProviderProfile> , IMedicalTestProviderProfileRepository
    {
        public MedicalTestProviderProfileRepository(ApplicationDbContext context) : base(context)
        {

        }

        public override async Task<IEnumerable<MedicalTestProviderProfile>> GetAllAsync()
        {
            return await _context.MedicalTestProviders
                .Include(m => m.User)
                .ToListAsync();
        }
        public override async Task<MedicalTestProviderProfile?> GetByIdAsync(int id)
        {
            return await _context.MedicalTestProviders
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
        public async Task<MedicalTestProviderProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.MedicalTestProviders
                .Include(m => m.User)
                .FirstOrDefaultAsync(m => m.UserId == userId);
        }

        public async Task<IEnumerable<MedicalTestProviderProfile>> GetApprovedProfilesAsync(MedicalTestProviderType? type = null)
        {
            var query = _context.MedicalTestProviders
                .Where(m => m.IsApproved);
            if (type.HasValue)
            {
                query = query.Where(m => m.Type == type.Value);
            }
            query = query.Include(m => m.User);
            return await query.OrderByDescending(M => M.Avg_Rating).ToListAsync();
        }

        public async Task<IEnumerable<MedicalTestProviderProfile>> GetProfilesByTypeAsync(MedicalTestProviderType type)
        {
            var profiles = await _context.MedicalTestProviders
                .Where(m => m.Type == type)
                .Include(m => m.User)
                .ToListAsync();
            return profiles;
        }
    }
}
