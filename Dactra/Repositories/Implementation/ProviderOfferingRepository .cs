using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class ProviderOfferingRepository : IProviderOfferingRepository
    {
        private readonly ApplicationDbContext _context;

        public ProviderOfferingRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public  async Task AddAsync(ProviderOffering entity)
        {
            await _context.ProviderOfferings.AddAsync(entity);
        }

        public void Delete(ProviderOffering entity)
        {
            _context.ProviderOfferings.Remove(entity);
        }

        public async Task<IEnumerable<ProviderOffering>> GetAllAsync()
        {
            return await _context.ProviderOfferings
         .Include(x => x.Provider)
         .Include(x => x.TestService)
         .ToListAsync();
        }

        public async Task<ProviderOffering?> GetByIdAsync(int id)
        {
            return await _context.ProviderOfferings
         .Include(x => x.Provider)
         .Include(x => x.TestService)
         .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<IEnumerable<ProviderOffering>> GetByProviderIdAsync(int providerId)
        {
                return await _context.ProviderOfferings
          .Where(x => x.ProviderId == providerId)
          .Include(x => x.TestService)
          .ToListAsync();
        }

        public async Task<IEnumerable<ProviderOffering>> GetByServiceIdAsync(int serviceId)
        {
            return await _context.ProviderOfferings
          .Where(x => x.TestServiceId == serviceId)
          .Include(x => x.Provider)
          .ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(ProviderOffering entity)
        {
            _context.ProviderOfferings.Update(entity);
        }
    }
}
