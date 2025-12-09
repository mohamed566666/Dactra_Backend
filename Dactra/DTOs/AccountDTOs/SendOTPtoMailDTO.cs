namespace Dactra.DTOs.AuthemticationDTOs
{
    public class SendOTPtoMailDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
