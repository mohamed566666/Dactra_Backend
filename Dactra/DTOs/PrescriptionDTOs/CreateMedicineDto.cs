namespace Dactra.DTOs.PrescriptionDTOs
{
    public class CreateMedicineDto
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string? Dose { get; set; } = string.Empty;

        [Required]
        [Range(1, 365, ErrorMessage = "Duration must be between 1 and 365 days")]
        public int Duration { get; set; }

        [Required]
        public TimesPerDay TimesPerDay { get; set; }

        [Required]
        public WhenToTake WhenToTake { get; set; }

        [Required]
        public string FirstDoseTime { get; set; } = string.Empty;
    }
}
