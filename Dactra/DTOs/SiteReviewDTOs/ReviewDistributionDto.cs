namespace Dactra.DTOs.SiteReviewDTOs
{
    public class ReviewDistributionDto
    {
        public int TotalReviews { get; set; }
        public decimal AverageScore { get; set; }
        public List<ReviewScoreStatDto> Scores { get; set; } = new();
    }
}
