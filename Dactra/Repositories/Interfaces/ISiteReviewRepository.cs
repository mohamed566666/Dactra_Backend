namespace Dactra.Repositories.Interfaces
{
    public interface ISiteReviewRepository : IGenericRepository<SiteReview>
    {
        Task<SiteReview?> GetByUserIdAsync(string userId);
        Task DeleteByIdAsync(int id);
        Task<int> GetCountAsync();
        Task<decimal> GetAverageScoreAsync();
        Task<IEnumerable<(int Score, int Count)>> GetScoreCountsAsync();
        ApplicationDbContext GetDbContext();
    }
}
