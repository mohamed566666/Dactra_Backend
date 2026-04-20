namespace Dactra.Helpers
{
    public static class FileValidationHelper
    {
        private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".jpg", ".jpeg", ".png", ".gif", ".webp" };

        private static readonly HashSet<string> AllowedDocumentExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".pdf", ".doc", ".docx" };

        public static (bool IsValid, string? ErrorMessage) ValidateFile(IFormFile file, long maxFileSizeBytes, string? category = null)
        {
            if (file == null || file.Length == 0)
                return (false, "File is Empty");

            if (file.Length > maxFileSizeBytes)
                return (false, $"File size exceeds the allowed limit ({maxFileSizeBytes / (1024 * 1024)} MB)");

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();

            var allowed = category?.ToLower() switch
            {
                "profile" or "doctor" or "patient" => AllowedImageExtensions,
                "qualification" or "document" => AllowedDocumentExtensions,
                _ => null
            };

            if (allowed != null && !allowed.Contains(ext))
                return (false, $"File type is not allowed for this category. Supported types: {string.Join(", ", allowed)}");

            return (true, null);
        }
    }
}