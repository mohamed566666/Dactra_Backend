using System.ComponentModel.DataAnnotations;

namespace Dactra.DTOs
{
    public class ResendOTPDto
    {
        [Required]
        public string Email { get; set; }
    }
}
