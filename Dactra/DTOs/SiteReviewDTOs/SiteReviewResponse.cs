namespace Dactra.DTOs.SiteReviewDTOs
{
    public class SiteReviewResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
        public string ReviewerName { get; set; } = string.Empty;
        public string? ReviewerImageUrl { get; set; }
    }
}
