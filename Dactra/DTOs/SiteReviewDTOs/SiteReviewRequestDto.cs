namespace Dactra.DTOs.SiteReviewDTOs
{
    public class SiteReviewRequestDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Score { get; set; }

        [MaxLength(2000)]
        public string? Comment { get; set; }
    }
}
