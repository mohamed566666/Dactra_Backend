using System.ComponentModel.DataAnnotations;

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
        [Range(1, 5)]
        public int Score { get; set; }
        [MaxLength(2000)]
        public string? Comment { get; set; }
        public bool IsVisible { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
