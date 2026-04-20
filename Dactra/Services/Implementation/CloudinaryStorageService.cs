using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Dactra.Services.Implementation
{
    public class CloudinaryStorageService : IFileStorageService
    {
        private readonly Cloudinary? _cloudinary;
        private readonly ILogger<CloudinaryStorageService> _logger;
        private readonly string _rootFolder;
        private readonly bool _isConfigured;

        public CloudinaryStorageService(IOptions<CloudinarySettings> settings, ILogger<CloudinarySettings> logger)
        {
            try
            {
                var s = settings.Value;

                if (string.IsNullOrWhiteSpace(s.CloudName) ||
                    string.IsNullOrWhiteSpace(s.ApiKey) ||
                    string.IsNullOrWhiteSpace(s.ApiSecret))
                {
                    _logger.LogError("❌ Cloudinary credentials are missing or invalid!");
                    _isConfigured = false;
                    _cloudinary = null;
                    _rootFolder = "Dactra";
                    return;
                }

                var account = new Account(s.CloudName, s.ApiKey, s.ApiSecret);
                _cloudinary = new Cloudinary(account);
                _logger.LogInformation("✅ Cloudinary client initialized for cloud: {CloudName}", s.CloudName);

                _isConfigured = true;
                _rootFolder = string.IsNullOrWhiteSpace(s.DefaultFolder) ? "Dactra" : s.DefaultFolder;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Failed to initialize Cloudinary client");
                _isConfigured = false;
                _cloudinary = null;
                _rootFolder = "Dactra";
            }
        }

        public async Task<(bool Success, string? Url, string? PublicId, string? Error)> UploadAsync(IFormFile file, string folder)
        {
            if (!_isConfigured || _cloudinary == null)
            {
                _logger.LogError("❌ Upload attempted but Cloudinary is not configured");
                return (false, null, null, "Cloudinary service is not configured properly");
            }

            try
            {
                if (file == null || file.Length == 0)
                    return (false, null, null, "No file provided");

                var stream = file.OpenReadStream();
                if (stream.CanSeek)
                    stream.Seek(0, SeekOrigin.Begin);

                var fileDescription = new FileDescription(file.FileName, stream);
                var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();

                UploadResult result;

                if (extension is ".jpg" or ".jpeg" or ".png" or ".gif" or ".webp" or ".svg")
                {
                    var uploadParams = new ImageUploadParams
                    {
                        File = fileDescription,
                        Folder = $"{_rootFolder}/{folder}",
                        Overwrite = false,
                        UniqueFilename = true,
                        UseFilename = false,
                        Transformation = new Transformation().Width(1920).Height(1920).Crop("limit")
                    };
                    result = await _cloudinary.UploadAsync(uploadParams);
                }
                else
                {
                    var uploadParams = new RawUploadParams
                    {
                        File = fileDescription,
                        Folder = $"{_rootFolder}/{folder}",
                        Overwrite = false,
                        UniqueFilename = true,
                        UseFilename = false
                    };
                    result = await _cloudinary.UploadAsync(uploadParams);
                }
                await stream.DisposeAsync();

                if (result.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    _logger.LogInformation("✅ Uploaded: {PublicId} | URL: {Url}", result.PublicId, result.SecureUrl);
                    return (true, result.SecureUrl?.AbsoluteUri, result.PublicId, null);
                }

                _logger.LogError("❌ Cloudinary API Error: {Error} | StatusCode: {Status}",
                    result.Error?.Message, result.StatusCode);
                return (false, null, null, result.Error?.Message ?? "Upload failed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "💥 Unexpected exception during upload");
                return (false, null, null, $"Unexpected error: {ex.Message}");
            }
        }

        public async Task<bool> DeleteAsync(string publicId)
        {
            if (!_isConfigured || _cloudinary == null || string.IsNullOrWhiteSpace(publicId))
                return false;

            try
            {
                var result = await _cloudinary.DestroyAsync(new DeletionParams(publicId));
                return result.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete {PublicId}", publicId);
                return false;
            }
        }
    }
}