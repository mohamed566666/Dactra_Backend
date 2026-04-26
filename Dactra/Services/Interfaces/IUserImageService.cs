namespace Dactra.Services.Interfaces
{
    public interface IUserImageService
    {
        Task<(bool Success, string? ImageUrl, string? Error)> UploadOrReplaceAsync(string userId, IFormFile file);
        Task<(bool Success, string? Error)> DeleteAsync(string userId);
        Task<UserImageResponseDTO?> GetAsync(string userId);
        Task<UserImageResponseDTO?> updateAsync(string userId, IFormFile file);
    }
}
