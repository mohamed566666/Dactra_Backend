namespace Dactra.Services.Interfaces
{
    public interface ISiteReviewService
    {
        public Task<SiteReview> CreateReviewAsync(string userId, SiteReviewRequestDto dto);
        public Task UpdateReviewAsync(string userId, int reviewId, SiteReviewRequestDto dto);
        public Task<IEnumerable<SiteReviewResponse>> GetAllReviewsAsync();
        public Task<(int Count, decimal Avg)> GetStatsAsync();
        public Task DeleteReviewAsync(string userId, int reviewId);
        public Task<ReviewDistributionDto> GetReviewDistributionAsync();
    }
}
