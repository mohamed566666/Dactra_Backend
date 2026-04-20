namespace Dactra.Services.Interfaces
{
    public interface IFileStorageService
    {
        Task<(bool Success, string? Url, string? PublicId, string? Error)> UploadAsync(IFormFile file, string folder);
        Task<bool> DeleteAsync(string publicId);
    }
}
