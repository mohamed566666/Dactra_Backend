namespace Dactra.DTOs.FileUpload
{
    public class FileUploadRequestDTO
    {
        public IFormFile File { get; set; } = null!;
        public string? Category { get; set; } = "general";
    }
}
