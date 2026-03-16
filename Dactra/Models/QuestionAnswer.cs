namespace Dactra.Models
{
    public class QuestionAnswer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Content { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        public bool isDeleted { get; set; } = false;

        [Required]
        public int QuestionId { get; set; }
        [ForeignKey(nameof(QuestionId))]
        public Question Question { get; set; } = null!;

        [Required]
        public int DoctorId { get; set; }
        [ForeignKey(nameof(DoctorId))]
        public DoctorProfile Doctor { get; set; } = null!;
        public int? ParentAnswerId { get; set; }
        [ForeignKey(nameof(ParentAnswerId))]
        public QuestionAnswer? ParentAnswer { get; set; }
        public ICollection<QuestionAnswer> Replies { get; set; } = new List<QuestionAnswer>();
    }
}
