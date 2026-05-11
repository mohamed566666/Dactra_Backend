namespace Dactra.DTOs
{
    public class MedicalReportResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Summary { get; set; }
        public DateTime UploadedAt { get; set; }
        public List<MedicalReportFileResponseDTO> Files { get; set; } = new();
    }
}
