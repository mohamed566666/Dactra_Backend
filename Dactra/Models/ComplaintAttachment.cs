using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dactra.Models
{
    public class ComplaintAttachment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ComplaintId { get; set; }

        [ForeignKey(nameof(ComplaintId))]
        public Complaint Complaint { get; set; }

        [Required]
        public string FileName { get; set; } = string.Empty;

        [Required]
        public string FilePath { get; set; } = string.Empty;

        public string? FileType { get; set; }
        public long FileSize { get; set; }

        public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
    }
}
