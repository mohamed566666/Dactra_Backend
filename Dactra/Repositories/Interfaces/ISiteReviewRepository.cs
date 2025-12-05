using Dactra.Models;

namespace Dactra.Repositories.Interfaces
{
    public interface ISiteReviewRepository
    {
        public Task AddAsync(SiteReview review);
        public Task<SiteReview?> GetByIdAsync(int id);
        public Task<SiteReview?> GetByUserIdAsync(string userId);
        public Task<IEnumerable<SiteReview>> GetAllAsync();
        public Task UpdateAsync(SiteReview review);
        public Task DeleteAsync(int id);
        public Task<int> GetCountAsync();
        public Task<decimal> GetAverageScoreAsync();
        public Task<IEnumerable<(int Score, int Count)>> GetScoreCountsAsync();
    }
}
