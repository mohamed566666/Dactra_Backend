namespace Dactra.DTOs.FileUpload
{
    public class FileUploadResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? FileUrl { get; set; }
        public string? PublicId { get; set; }
        public string? ResourceType { get; set; }
    }
}
