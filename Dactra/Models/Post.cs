namespace Dactra.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        public string? ImageUrl { get; set; }
        public string? ImagePublicId { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int DoctorId { get; set; }

        [ForeignKey(nameof(DoctorId))]
        public DoctorProfile Doctor { get; set; } = null!;
        public DateTime? UpdatedAt { get; set; }
        public bool isDeleted { get; set; } = false;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<PostLike> Likes { get; set; } = new List<PostLike>();
        public ICollection<SavedPost> SavedBy { get; set; } = new List<SavedPost>();
        public ICollection<PostTag> PostTags { get; set; } = new List<PostTag>();

    }
}
