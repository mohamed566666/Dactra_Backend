using Dactra.DTOs.AuthemticationDTOs;
using Dactra.Factories.Interfaces;
using Dactra.Models;
using Dactra.Repositories.Implementation;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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
        private readonly ApplicationDbContext _context;

        public UserService(IUserProfileFactory userProfileFactory, IUserRepository userRepository, IEmailSender emailSender, IEmailVerificationRepository emailVerificationRepository, UserManager<ApplicationUser> userManager , IRoleRepository roleRepository, ApplicationDbContext context)
        {
            _UserProfileFactory = userProfileFactory;
            _userRepository = userRepository;
            _emailSender = emailSender;
            _emailVerificationRepository = emailVerificationRepository;
            _userManager = userManager;
            _roleRepository = roleRepository;
            _context = context;
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
            await using var transAction = await _context.Database.BeginTransactionAsync();
            try
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
                    IsRegistrationComplete = false
                };
                var createUserResult = await _userRepository.CreateUserAsync(user, model.Password);
                if (!createUserResult.Succeeded)
                {
                    return createUserResult;
                }
                string verificationCode = new Random().Next(100000, 999999).ToString();
                await _emailSender.SendEmailAsync(model.Email, "Verification Code", $"Your OTP is: <b>{verificationCode}</b>");
                await _emailVerificationRepository.AddVerificationAsync(model.Email, verificationCode, TimeSpan.FromMinutes(5));
                model.Role = model.Role.ToLower();
                var roleName = model.Role switch
                {
                    "doctor" => "DoctorProfile",
                    "patient" => "PatientProfile",
                    "lab" => "MedicalTestProviderProfile",
                    "scan" => "MedicalTestProviderProfile",
                    _ => model.Role
                };
                await _roleRepository.AddUserToRoleAsync(user, roleName);
                await transAction.CommitAsync();
                return createUserResult;
            }
            catch (Exception ex)
            {
                await transAction.RollbackAsync();
                return IdentityResult.Failed(new IdentityError
                {
                    Description = $"Registration failed: {ex.Message}"
                });
            }
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }
        public async Task UpdateAsync(ApplicationUser user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
