namespace Dactra.Models
{
    public class MedicalReport
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string FileUrl { get; set; } = string.Empty;

        [Required]
        public string PublicId { get; set; } = string.Empty;

        [MaxLength(20)]
        public string FileType { get; set; } = string.Empty;

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

        public int PatientProfileId { get; set; }

        [ForeignKey(nameof(PatientProfileId))]
        public PatientProfile PatientProfile { get; set; } = null!;
    }
}
