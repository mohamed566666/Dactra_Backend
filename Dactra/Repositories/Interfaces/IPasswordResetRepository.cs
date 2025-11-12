using Dactra.DTOs.AccountDTOs;

namespace Dactra.Repositories.Interfaces
{
    public interface IPasswordResetRepository
    {
        Task<bool> ResetPasswordUsingRefreshTokenAsync(ResetPasswordTokenDto model);
    }
}
