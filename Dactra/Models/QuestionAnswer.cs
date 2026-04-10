namespace Dactra.Models
{
    public class QuestionAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
        public DateTimeOffset? UpdatedAt { get; set; }
        public bool isDeleted { get; set; } = false;

        [Required]
        public int QuestionId { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public Question Question { get; set; } = null!;

        [Required]
        public string AnswererUserId { get; set; } = string.Empty;
        public ApplicationUser Answerer { get; set; } = null!;

        public int? ParentAnswerId { get; set; }
        [ForeignKey(nameof(ParentAnswerId))]
        public QuestionAnswer? ParentAnswer { get; set; }

        public ICollection<QuestionAnswer> Replies { get; set; } = new List<QuestionAnswer>();
        public ICollection<QuestionAnswerLike> Likes { get; set; } = new List<QuestionAnswerLike>();
    }
}
