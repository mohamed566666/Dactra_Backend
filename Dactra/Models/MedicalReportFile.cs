namespace Dactra.Models
{
    public class MedicalReportFile
    {
        public int Id { get; set; }
        [Required]
        public string FileUrl { get; set; } = string.Empty;
        [Required]
        public string PublicId { get; set; } = string.Empty;
        [MaxLength(20)]
        public string FileType { get; set; } = string.Empty;

        public int MedicalReportId { get; set; }
        [ForeignKey(nameof(MedicalReportId))]
        public MedicalReport MedicalReport { get; set; } = null!;
    }
}
