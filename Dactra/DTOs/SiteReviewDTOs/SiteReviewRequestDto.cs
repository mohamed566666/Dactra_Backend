namespace Dactra.DTOs.SiteReviewDTOs
{
    public class SiteReviewRequestDto
    {
        [Required]
        [Range(1, 5)]
        public int Score { get; set; }

        [MaxLength(2000)]
        public string? Comment { get; set; }
    }
}
