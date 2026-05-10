namespace Dactra.DTOs
{
    public class UploadMedicalReportDTO
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; } = string.Empty;
        public string? Summary { get; set; }

        [Required]
        public List<IFormFile> Files { get; set; } = new();
    }
}
