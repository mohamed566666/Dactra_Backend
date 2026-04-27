namespace Dactra.Models
{
    public class SiteReview
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ReviewerUserId { get; set; } = null!;
        public ApplicationUser? Reviewer { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Range(1, 5)]
        public int Score { get; set; }

        [MaxLength(2000)]
        public string? Comment { get; set; }

        [MaxLength(100)]
        public string ReviewerName { get; set; } = string.Empty;

        [MaxLength(500)]
        public string? ReviewerImageUrl { get; set; }

        public bool IsVisible { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
