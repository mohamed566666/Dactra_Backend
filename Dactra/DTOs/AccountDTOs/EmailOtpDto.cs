namespace Dactra.DTOs.AuthemticationDTOs
{
    public class EmailOtpDto
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string OTP { get; set; } = string.Empty;
        public DateTime ExpiryDate { get; set; }
    }
}
