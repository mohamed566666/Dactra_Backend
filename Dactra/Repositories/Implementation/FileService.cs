using Dactra.DTOs.FileUpload;

namespace Dactra.Repositories.Implementation
{
    public class FileService : IFileService
    {
        private readonly IFileStorageService _storage;
        private readonly ILogger<FileService> _logger;

        public FileService(IFileStorageService storage, ILogger<FileService> logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public async Task<FileUploadResponseDTO> UploadAsync(IFormFile file, string category, long maxFileSizeMB)
        {
            var maxBytes = maxFileSizeMB * 1024 * 1024;
            var (isValid, error) = FileValidationHelper.ValidateFile(file, maxBytes, category);

            if (!isValid)
                return new FileUploadResponseDTO { Success = false, Message = error };

            var (success, url, publicId, uploadError) = await _storage.UploadAsync(file, category);

            if (!success)
                return new FileUploadResponseDTO { Success = false, Message = uploadError };

            var ext = Path.GetExtension(file.FileName)?.ToLower();
            var resourceType = ext is ".pdf" or ".doc" or ".docx" ? "raw" : "image";

            return new FileUploadResponseDTO
            {
                Success = true,
                Message = "File uploaded Succesfully",
                FileUrl = url,
                PublicId = publicId,
                ResourceType = resourceType
            };
        }

        public async Task<bool> DeleteAsync(string publicId)
            => await _storage.DeleteAsync(publicId);
    }
}
