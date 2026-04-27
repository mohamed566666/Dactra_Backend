namespace Dactra.DTOs
{
    public class UploadMedicalReportDTO
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
