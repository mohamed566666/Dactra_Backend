namespace Dactra.Services.Implementation
{
    public class SiteReviewService : ISiteReviewService
    {
        private readonly ISiteReviewRepository _repo;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public SiteReviewService(
            ISiteReviewRepository repo,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context
            )
        {
            _repo = repo;
            _mapper = mapper;
            _userManager = userManager;
            _context = context;
        }

        public async Task<SiteReview> CreateReviewAsync(string userId, SiteReviewRequestDto dto)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User id is required", nameof(userId));

            var existing = await _repo.GetByUserIdAsync(userId);
            if (existing != null)
                throw new InvalidOperationException("User already submitted a review. Use update instead.");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                throw new InvalidOperationException("User not found");

            var review = _mapper.Map<SiteReview>(dto);
            review.ReviewerUserId = userId;
            review.CreatedAt = DateTime.UtcNow;
            review.ReviewerName = GetUserDisplayName(user);
            review.ReviewerImageUrl = user.ImageUrl;
            await _repo.AddAsync(review);
            await _repo.SaveChangesAsync();

            return review;
        }

        public async Task UpdateReviewAsync(string userId, int reviewId, SiteReviewRequestDto dto)
        {
            var existing = await _repo.GetByIdAsync(reviewId);
            if (existing == null)
                throw new KeyNotFoundException($"Review with id {reviewId} not found.");

            if (existing.ReviewerUserId != userId)
                throw new UnauthorizedAccessException("You are not allowed to edit this review.");

            existing.Title = dto.Title;
            existing.Score = dto.Score;
            existing.Comment = dto.Comment;
            existing.UpdatedAt = DateTime.UtcNow;
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                existing.ReviewerName = GetUserDisplayName(user);
                existing.ReviewerImageUrl = user.ImageUrl;
            }

            _repo.Update(existing);
            await _repo.SaveChangesAsync();
        }

        public async Task<IEnumerable<SiteReviewResponse>> GetAllReviewsAsync()
        {
            var reviews = await _repo.GetAllAsync();
            var response = reviews.Select(r =>
            {
                var res = _mapper.Map<SiteReviewResponse>(r);
                if (string.IsNullOrEmpty(res.ReviewerName))
                    res.ReviewerName = "Anonymous";
                return res;
            });
            return response;
        }

        private string GetUserDisplayName(ApplicationUser user)
        {

            var scope = _repo.GetType().GetMethod("GetPatientName")?.Invoke(null, null);

            var patient = _context.Patients
                .FirstOrDefault(p => p.UserId == user.Id);

            if (patient != null)
                return $"{patient.FirstName} {patient.LastName}";

            var doctor = _context.Doctors
                .FirstOrDefault(d => d.UserId == user.Id);

            if (doctor != null)
                return $"{doctor.FirstName} {doctor.LastName}";

            return !string.IsNullOrEmpty(user.UserName) ? user.UserName : user.Email?.Split('@')[0] ?? "User";
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

        public async Task<(int Count, decimal Avg)> GetStatsAsync()
        {
            var count = await _repo.GetCountAsync();
            var avg = await _repo.GetAverageScoreAsync();
            return (count, avg);
        }
    }
}
