namespace Dactra.Models
{
    public class Questions
    {
        [Key]
        public int  Id { get; set; }

        [Required]
        public string Text { get; set; } = string.Empty;

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public int MajorId { get; set; }

        [ForeignKey(nameof(MajorId))]
        public Majors Major { get; set; } = null!;

        [Required]
        public int PatientId { get; set; }

        [ForeignKey(nameof(PatientId))]
        public PatientProfile Patient { get; set; } = null!;
        public List<Answer> Answers { get; set; } = new List<Answer>();
        public bool isDeleted { get; set; } = false;


    }
}
