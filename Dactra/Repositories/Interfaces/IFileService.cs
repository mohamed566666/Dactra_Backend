using Dactra.DTOs.FileUpload;

namespace Dactra.Repositories.Interfaces
{
    public interface IFileService
    {
        Task<FileUploadResponseDTO> UploadAsync(IFormFile file, string category, long maxFileSizeMB);
        Task<bool> DeleteAsync(string publicId);
    }
}
