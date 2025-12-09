namespace Dactra.Repositories.Interfaces
{
    public interface ISiteReviewRepository : IGenericRepository<SiteReview>
    {
        public Task<SiteReview?> GetByUserIdAsync(string userId);
        public Task DeleteByIdAsync(int id);
        public Task<int> GetCountAsync();
        public Task<decimal> GetAverageScoreAsync();
        public Task<IEnumerable<(int Score, int Count)>> GetScoreCountsAsync();
    }
}
