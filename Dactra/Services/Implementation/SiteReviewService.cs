namespace Dactra.Services.Implementation
{
    public class SiteReviewService : ISiteReviewService
    {
        private readonly ISiteReviewRepository _repo;
        private readonly IMapper _mapper;

        public SiteReviewService(ISiteReviewRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<SiteReview> CreateReviewAsync(string userId, SiteReviewRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User id is required", nameof(userId));
            var existing = await _repo.GetByUserIdAsync(userId);
            if (existing != null)
                throw new InvalidOperationException("User already submitted a review. Use update instead.");
            var review = _mapper.Map<SiteReview>(dto);
            review.ReviewerUserId = userId;
            review.CreatedAt = DateTime.UtcNow;
            await _repo.AddAsync(review);
            return review;
        }

        public async Task UpdateReviewAsync(string userId, int reviewId, SiteReviewRequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(reviewId);
            if (existing == null)
                throw new KeyNotFoundException($"Review with id {reviewId} not found.");

            if (existing.ReviewerUserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to edit this review.");
            _mapper.Map(dto, existing);
            existing.UpdatedAt = DateTime.UtcNow;
            _repo.Update(existing);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<SiteReviewResponse>> GetAllReviewsAsync()
        {
            var reviews = await _repo.GetAllAsync();
            return _mapper.Map<IEnumerable<SiteReviewResponse>>(reviews);
        }

        public async Task<(int Count, decimal Avg)> GetStatsAsync()
        {
            var count = await _repo.GetCountAsync();
            var avg = await _repo.GetAverageScoreAsync();
            return (count, avg);
        }

        public async Task DeleteReviewAsync(string userId, int reviewId)
        {
            var existing = await _repo.GetByIdAsync(reviewId);
            if (existing == null)
                throw new KeyNotFoundException($"Review with id {reviewId} not found.");
            if (existing.ReviewerUserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to delete this review.");
            await _repo.DeleteByIdAsync(reviewId);
        }

        public async Task<ReviewDistributionDto> GetReviewDistributionAsync()
        {
            var grouped = await _repo.GetScoreCountsAsync();
            var total = grouped.Sum(x => x.Count);
            decimal average = 0m;
            if (total > 0)
            {
                var avgVal = await _repo.GetAverageScoreAsync();
                average = Math.Round(avgVal, 2);
            }

            var result = new ReviewDistributionDto
            {
                TotalReviews = total,
                AverageScore = average
            };
            for (int s = 1; s <= 5; s++)
            {
                var found = grouped.FirstOrDefault(g => g.Score == s);
                int count = found == default ? 0 : found.Count;
                decimal percent = total == 0 ? 0m : Math.Round((decimal)count * 100m / total, 2);
                result.Scores.Add(new ReviewScoreStatDto
                {
                    Score = s,
                    Count = count,
                    Percentage = percent
                });
            }
            return result;
        }
    }
}
