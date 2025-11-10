using System.ComponentModel.DataAnnotations;

namespace Dactra.Models
{
    public class EmailVerification
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public string OTP { get; set; } = string.Empty;

        [Required]
        public DateTime ExpiryDate { get; set; } = DateTime.UtcNow.AddMinutes(15);
    }
}
