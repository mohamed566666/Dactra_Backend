namespace Dactra.DTOs
{
    public class MedicalReportFileResponseDTO
    {
        public int Id { get; set; }
        public string FileUrl { get; set; } = string.Empty;
        public string FileType { get; set; } = string.Empty;
    }
}
