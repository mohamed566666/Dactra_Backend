namespace Dactra.Services.Interfaces
{
    public interface IUserService
    {
        public Task<IdentityResult> SendDTOforVerficatio(SendOTPtoMailDTO model);
        public Task<IdentityResult> RegisterAsync(RegisterDto model);
        public Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task UpdateAsync(ApplicationUser user);
        Task <ApplicationUser> LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal);
        public Task ChangePasswordAsync(string userId , ChangePasswordRequestDto model);

    }
}
