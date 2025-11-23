using Dactra.DTOs;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;

namespace Dactra.Services.Implementation
{
    public class ProviderOfferingService : IProviderOfferingService
    {
        private readonly IProviderOfferingRepository _repo;

        public ProviderOfferingService(IProviderOfferingRepository repo)
        {
            _repo = repo;
        }
        public async Task<ProviderOffering> CreateAsync(ProviderOfferingDto dto)
        {
            var entity = new ProviderOffering
            {
                ProviderId = dto.ProviderId,
                TestServiceId = dto.TestServiceId,
                Price = dto.Price,
                Duration = dto.Duration
            };

            await _repo.AddAsync(entity);
            await _repo.SaveAsync();

            return entity;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return false;

            _repo.Delete(entity);
            await _repo.SaveAsync();

            return true;
        }

        public Task<IEnumerable<ProviderOffering>> GetAllAsync()
        {
           return _repo.GetAllAsync();
        }

        public Task<ProviderOffering?> GetByIdAsync(int id)
        {
            return (_repo.GetByIdAsync(id));
        }

        public Task<IEnumerable<ProviderOffering>> GetByProviderIdAsync(int providerId)
        {
            return _repo.GetByProviderIdAsync(providerId);
        }

        public Task<IEnumerable<ProviderOffering>> GetByServiceIdAsync(int serviceId)
        {
           return _repo.GetByServiceIdAsync(serviceId);
        }

        public async Task<ProviderOffering?> UpdateAsync(int id, ProviderOfferingDto dto)
        {
            var entity = await _repo.GetByIdAsync(id);
            if (entity == null) return null;

            entity.ProviderId = dto.ProviderId;
            entity.TestServiceId = dto.TestServiceId;
            entity.Price = dto.Price;
            entity.Duration = dto.Duration;

            _repo.Update(entity);
            await _repo.SaveAsync();

            return entity;
        }
    }
}
