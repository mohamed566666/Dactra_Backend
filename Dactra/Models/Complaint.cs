namespace Dactra.Models
{
    public class Complaint
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public required string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }
        [Required]
        public string Title { get; set; }
        [Required]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? ResolvedAt { get; set; }
        public ComplaintStatus status { get; set; } = ComplaintStatus.Pending;
        public string? AdminId { get; set; }
        public string? AdminResponse { get; set; }
        public ICollection<ComplaintAttachment> Attachments { get; set; } = new List<ComplaintAttachment>();
    }
}
