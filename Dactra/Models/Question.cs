namespace Dactra.Models
{
    public class Question
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Content { get; set; } = string.Empty;
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        [Required]
        public int PatientId { get; set; }
        [ForeignKey(nameof(PatientId))]
        public PatientProfile Patient { get; set; } = null!;
        public bool isDeleted { get; set; } = false;
        public ICollection<QuestionAnswer> Answers { get; set; } = new List<QuestionAnswer>();
        public ICollection<QuestionInterest> Interests { get; set; } = new List<QuestionInterest>();
        public ICollection<QuestionSave> SavedBy { get; set; } = new List<QuestionSave>();
        public ICollection<QuestionTag> QuestionTags { get; set; } = new List<QuestionTag>();
    }
}
