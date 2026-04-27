namespace Dactra.DTOs
{
    public class MedicalReportResponseDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
        public DateTime UploadedAt { get; set; }
    }
}
