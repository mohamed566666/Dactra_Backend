using System.ComponentModel.DataAnnotations;

namespace Dactra.DTOs
{
    public class SendOTPtoMailDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
