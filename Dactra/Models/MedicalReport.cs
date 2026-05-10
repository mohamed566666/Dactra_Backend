namespace Dactra.Models
{
    public class MedicalReport
    {
        public int Id { get; set; }
        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
        public int PatientProfileId { get; set; }
        [ForeignKey(nameof(PatientProfileId))]
        public PatientProfile PatientProfile { get; set; } = null!;
        public ICollection<MedicalReportFile> Files { get; set; } = new List<MedicalReportFile>();
    }
}
