namespace Dactra.DTOs.AccountDTOs
{
    public class ChangePasswordRequestDto
    {
        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 8 characters")]
        public string NewPassword { get; set; } = string.Empty;

        [Required]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords must match")]
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
}
