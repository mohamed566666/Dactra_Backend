namespace Dactra.Services.Implementation
{
    public class ServiceProviderService : IServiceProviderService
    {
        private readonly IServiceProviderRepository _repository;

        public ServiceProviderService(IServiceProviderRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> ApproveAsync(ServiceProviderType type, int id)
        {
            var provider = await _repository.GetByIdAsync(type, id);
            if (provider == null)
                return false;
            provider.IsApproved = true;
            await _repository.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DisApproveAsync(ServiceProviderType type, int id)
        {
            var provider = await _repository.GetByIdAsync(type, id);
            if (provider == null)
                return false;
            provider.IsApproved = false;
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
