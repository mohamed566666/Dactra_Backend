namespace Dactra.Services.Interfaces
{
    public interface ISiteReviewService
    {
        Task<SiteReview> CreateReviewAsync(string userId, SiteReviewRequestDto dto);
        Task UpdateReviewAsync(string userId, int reviewId, SiteReviewRequestDto dto);
        Task<IEnumerable<SiteReviewResponse>> GetAllReviewsAsync();
        Task<(int Count, decimal Avg)> GetStatsAsync();
        Task DeleteReviewAsync(string userId, int reviewId);
        Task<ReviewDistributionDto> GetReviewDistributionAsync();
    }
}
