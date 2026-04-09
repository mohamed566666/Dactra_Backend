using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using System.Net;

namespace Dactra.Services.Implementation
{
    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(IOptions<CloudinarySettings> config)
        {
            var settings = config.Value;

            var account = new Account(
                settings.CloudName,
                settings.ApiKey,
                settings.ApiSecret
            );

            _cloudinary = new Cloudinary(account);
            _cloudinary.Api.Secure = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                                         | SecurityProtocolType.Tls13;
        }

        public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile file)
        {
            var uploadResult = new ImageUploadResult();

            if (file.Length <= 0)
                return uploadResult;

            try
            {
                using var stream = file.OpenReadStream();

                var uploadParams = new ImageUploadParams
                {
                    File = new FileDescription(file.FileName, stream),
                    Folder = "dactra-photos"
                };

                uploadResult = await _cloudinary.UploadAsync(uploadParams);

                Console.WriteLine($"Upload result: {uploadResult.StatusCode}");
                Console.WriteLine($"Error: {uploadResult.Error?.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"=== UPLOAD CRASH ===");
                Console.WriteLine(ex.ToString());
                throw;
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            return await _cloudinary.DestroyAsync(deleteParams);
        }
    }
}
