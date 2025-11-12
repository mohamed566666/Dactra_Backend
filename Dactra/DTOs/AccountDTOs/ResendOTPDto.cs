using System.ComponentModel.DataAnnotations;

namespace Dactra.DTOs.AuthemticationDTOs
{
    public class ResendOTPDto
    {
        [Required]
        public string Email { get; set; }
    }
}
