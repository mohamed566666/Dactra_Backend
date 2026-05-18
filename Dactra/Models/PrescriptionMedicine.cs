namespace Dactra.Models
{
    public class PrescriptionMedicine
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PrescriptionId { get; set; }

        [ForeignKey(nameof(PrescriptionId))]
        public Prescription Prescription { get; set; } = null!;

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        public string? Dose { get; set; }

        [Required]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
        public int Duration { get; set; }

        [Required]
        public TimesPerDay TimesPerDay { get; set; }

        [Required]
        public WhenToTake WhenToTake { get; set; }

        [Required]
        public TimeSpan FirstDoseTime { get; set; }

        public ICollection<MedicineDoseTime> DoseTimes { get; set; } = new List<MedicineDoseTime>();
    }
}
