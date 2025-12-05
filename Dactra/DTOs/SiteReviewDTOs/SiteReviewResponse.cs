namespace Dactra.DTOs.SiteReviewDTOs
{
    public class SiteReviewResponse
    {
        public int Id { get; set; }
        public int Score { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
