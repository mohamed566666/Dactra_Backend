using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class ProviderOfferingRepository : GenericRepository<ProviderOffering>  , IProviderOfferingRepository
    {
        public ProviderOfferingRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<IEnumerable<ProviderOffering>> GetAllAsync()
        {
            return await _context.ProviderOfferings
         .Include(x => x.Provider)
         .Include(x => x.TestService)
         .ToListAsync();
        }

        public override async Task<ProviderOffering?> GetByIdAsync(int id)
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
    }
}
