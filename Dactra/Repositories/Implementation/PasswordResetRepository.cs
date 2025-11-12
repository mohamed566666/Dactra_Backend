using Dactra.DTOs.AccountDTOs;
using Dactra.Models;
using Dactra.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Dactra.Repositories.Implementation
{
    public class PasswordResetRepository : IPasswordResetRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;

        public PasswordResetRepository(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }
        public async Task<bool> ResetPasswordUsingRefreshTokenAsync(ResetPasswordTokenDto model)
        {
            if (model.NewPassword != model.ConfirmPassword)
                return false;

            var tokenEntity = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(t => t.Token == model.RefreshToken && !t.IsUsed);

            if (tokenEntity == null || tokenEntity.ExpireAt < DateTime.UtcNow)
                return false;

            var user = await _userManager.FindByIdAsync(tokenEntity.UserId);
            if (user == null)
                return false;

            
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);

            if (!result.Succeeded)
                return false;

            tokenEntity.IsUsed = true;
            _context.UserRefreshTokens.Update(tokenEntity);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
