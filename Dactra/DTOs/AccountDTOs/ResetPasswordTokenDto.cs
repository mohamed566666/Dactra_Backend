namespace Dactra.DTOs.AccountDTOs
{
    public class ResetPasswordTokenDto
    {
        public string RefreshToken { get; set; }
        public string NewPassword { get; set; }
        public string ConfirmPassword { get; set; }
    }
}
