using Dactra.DTOs.SiteReviewDTOs;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;

namespace Dactra.Services.Implementation
{
    public class SiteReviewService : ISiteReviewService
    {
        private readonly ISiteReviewRepository _repo;

        public SiteReviewService(ISiteReviewRepository repo)
        {
            _repo = repo;
        }

        public async Task<SiteReview> CreateReviewAsync(string userId, SiteReviewRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User id is required", nameof(userId));
            var existing = await _repo.GetByUserIdAsync(userId);
            if (existing != null)
                throw new InvalidOperationException("User already submitted a review. Use update instead.");

            var review = new SiteReview
            {
                ReviewerUserId = userId,
                Score = dto.Score,
                Comment = dto.Comment,
                CreatedAt = DateTime.UtcNow
            };

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

            existing.Score = dto.Score;
            existing.Comment = dto.Comment;
            existing.UpdatedAt = DateTime.UtcNow;

            await _repo.UpdateAsync(existing);
        }

        public async Task<IEnumerable<SiteReviewResponse>> GetAllReviewsAsync()
        {
            var reviews = await _repo.GetAllAsync();
            return reviews.Select(r => new SiteReviewResponse
            {
                Id = r.Id,
                Score = r.Score,
                Comment = r.Comment,
                CreatedAt = r.CreatedAt,
            });
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

            await _repo.DeleteAsync(reviewId);
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
