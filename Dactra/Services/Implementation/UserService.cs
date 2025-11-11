using Dactra.DTOs;
using Dactra.Factories.Interfaces;
using Dactra.Models;
using Dactra.Repositories.Implementation;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Dactra.Services.Implementation
{
    public class UserService : IUserService
    {
        private readonly IUserProfileFactory _UserProfileFactory;
        private readonly IUserRepository _userRepository;
        private readonly IEmailSender _emailSender;
        private readonly IEmailVerificationRepository _emailVerificationRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRoleRepository _roleRepository;

        public UserService(IUserProfileFactory userProfileFactory, IUserRepository userRepository, IEmailSender emailSender, IEmailVerificationRepository emailVerificationRepository, UserManager<ApplicationUser> userManager , IRoleRepository roleRepository)
        {
            _UserProfileFactory = userProfileFactory;
            _userRepository = userRepository;
            _emailSender = emailSender;
            _emailVerificationRepository = emailVerificationRepository;
            _userManager = userManager;
            _roleRepository = roleRepository;
        }
        public async Task<IdentityResult> SendDTOforVerficatio(SendOTPtoMailDTO model)
        {
            var user = await _userRepository.GetUserByEmailAsync(model.Email);
            if (user == null) 
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }
            string verificationCode = new Random().Next(100000, 999999).ToString();
            await _emailSender.SendEmailAsync(model.Email, "Verification Code To Dactra", $"Your OTP is: <b>{verificationCode}</b>");
            await _emailVerificationRepository.AddVerificationAsync(model.Email, verificationCode, TimeSpan.FromMinutes(5));
            return IdentityResult.Success;
        }

        public async Task<IdentityResult> RegisterAsync(RegisterDto model)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Email is already registered." });
            }
            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow,
                IsActive = false,
                IsVerified = false,
            };
            var createUserResult = await _userRepository.CreateUserAsync(user, model.Password);
            if (!createUserResult.Succeeded)
            {
                return createUserResult;
            }
            var UserProfile = _UserProfileFactory.CreateProfile(model.Role, user.Id);
            UserProfile.UserId = user.Id;
            UserProfile.User = user;
            string verificationCode = new Random().Next(1000, 9999).ToString();
            await _emailSender.SendEmailAsync(model.Email, "Verification Code", $"Your OTP is: <b>{verificationCode}</b>");
            await _emailVerificationRepository.AddVerificationAsync(model.Email, verificationCode, TimeSpan.FromMinutes(1));
            await _roleRepository.AddUserToRoleAsync(user, model.Role);
            return createUserResult;
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }
    }
}
