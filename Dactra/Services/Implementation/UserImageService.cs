namespace Dactra.Services.Implementation
{
    public class UserImageService : IUserImageService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IFileStorageService _storage;
        private readonly ILogger<UserImageService> _logger;

        private const string Folder = "profile-images";

        public UserImageService(
            UserManager<ApplicationUser> userManager,
            IFileStorageService storage,
            ILogger<UserImageService> logger)
        {
            _userManager = userManager;
            _storage = storage;
            _logger = logger;
        }

        public async Task<(bool Success, string? ImageUrl, string? Error)> UploadOrReplaceAsync(string userId, IFormFile file)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, null, "User not found");

            if (!string.IsNullOrWhiteSpace(user.ImagePublicId))
            {
                var deleted = await _storage.DeleteAsync(user.ImagePublicId);
                if (!deleted)
                    _logger.LogWarning("⚠️ Could not delete old image: {PublicId}", user.ImagePublicId);
            }

            var (success, url, publicId, error) = await _storage.UploadAsync(file, Folder);
            if (!success)
                return (false, null, error);

            user.ImageUrl = url;
            user.ImagePublicId = publicId;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                await _storage.DeleteAsync(publicId!);
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, null, $"Failed to update user: {errors}");
            }

            return (true, url, null);
        }

        public async Task<(bool Success, string? Error)> DeleteAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            if (string.IsNullOrWhiteSpace(user.ImagePublicId))
                return (false, "User has no profile image");

            var deleted = await _storage.DeleteAsync(user.ImagePublicId);
            if (!deleted)
                return (false, "Failed to delete image from storage");

            user.ImageUrl = null;
            user.ImagePublicId = null;

            await _userManager.UpdateAsync(user);
            return (true, null);
        }

        public async Task<UserImageResponseDTO?> GetAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return null;

            return new UserImageResponseDTO
            {
                ImageUrl = user.ImageUrl,
                PublicId = user.ImagePublicId
            };
        }

        public async Task<UserImageResponseDTO> updateAsync(string UserId, IFormFile File)
        {
            var existingImage = await GetAsync(UserId);
            var user = await _userManager.FindByIdAsync(UserId);
            if (existingImage != null)
            {
                var deleteResult = await DeleteAsync(UserId);
                if (!deleteResult.Success)
                    throw new Exception($"Failed to delete existing image: {deleteResult.Error}");
            }
            var uploadResult = await UploadOrReplaceAsync(UserId, File);
            if (!uploadResult.Success)
                throw new Exception($"Failed to upload new image: {uploadResult.Error}");
            return new UserImageResponseDTO
            {
                ImageUrl = uploadResult.ImageUrl,
                PublicId = user.ImagePublicId
            };
        }
    }
}
