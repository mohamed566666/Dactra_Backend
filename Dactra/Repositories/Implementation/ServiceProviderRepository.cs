
namespace Dactra.Repositories.Implementation
{
    public class ServiceProviderRepository : IServiceProviderRepository
    {
        private readonly ApplicationDbContext _context;

        public ServiceProviderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceProviderProfile?> GetByIdAsync(int id)
        {
            ServiceProviderProfile? profile = await _context.Doctors
                .FirstOrDefaultAsync(sp => sp.Id == id);
            if (profile == null)
            {
                profile = await _context.MedicalTestProviders.FirstOrDefaultAsync(sp => sp.Id == id);   
            }
            return profile;
        }
        public async Task<ServiceProviderProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.Providers
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
