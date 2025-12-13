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
        private readonly ITokenService _tokenService;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserProfileFactory userProfileFactory, IUserRepository userRepository, IEmailSender emailSender, IEmailVerificationRepository emailVerificationRepository, UserManager<ApplicationUser> userManager, IRoleRepository roleRepository, ApplicationDbContext context, ITokenService tokenService, ILogger<UserService> logger)
        {
            _UserProfileFactory = userProfileFactory;
            _userRepository = userRepository;
            _emailSender = emailSender;
            _emailVerificationRepository = emailVerificationRepository;
            _userManager = userManager;
            _roleRepository = roleRepository;
            _context = context;
            _tokenService = tokenService;
            _logger = logger;
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
                    "doctor" => "Doctor",
                    "patient" => "Patient",
                    "lab" => "MedicalTestProvider",
                    "scan" => "MedicalTestProvider",
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

        public async Task<ApplicationUser> LoginWithGoogleAsync(ClaimsPrincipal? claimsPrincipal)
        {
            _logger.LogInformation("LoginWithGoogleAsync started.");

            if (claimsPrincipal == null)
            {
                _logger.LogError("claimsPrincipal is NULL");
                throw new ArgumentNullException(nameof(claimsPrincipal));
            }

            var email = claimsPrincipal.FindFirstValue(ClaimTypes.Email);

            if (email == null)
            {
                _logger.LogWarning("Email claim not found in Google response");
                throw new Exception("Email claim is null");
            }

            _logger.LogInformation("Google Email received: {Email}", email);

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                _logger.LogInformation("User not found. Creating new user...");

                var newUser = new ApplicationUser
                {
                    Email = email,
                    UserName = email,
                    IsVerified = false,
                    PhoneNumber = null,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.UtcNow,
                    IsActive = false,
                    IsRegistrationComplete = false
                };

                var result = await _userManager.CreateAsync(newUser);
                if (!result.Succeeded)
                {
                    _logger.LogError("Failed to create new user. Errors: {Errors}",
                        string.Join(", ", result.Errors.Select(e => e.Description)));

                    throw new Exception("Unable to create user");
                }

                _logger.LogInformation("New user created successfully: {Email}", newUser.Email);
                user = newUser;
            }
            else
            {
                _logger.LogInformation("Existing user found: {Email}", user.Email);
            }

            var providerKey = claimsPrincipal.FindFirstValue(ClaimTypes.Email) ?? string.Empty;

            var info = new UserLoginInfo("Google", providerKey, "Google");

            var loginResult = await _userManager.AddLoginAsync(user, info);

            if (!loginResult.Succeeded)
            {
                _logger.LogError("AddLoginAsync failed for user {Email}", user.Email);
                throw new Exception("Unable to login user with Google");
            }

            _logger.LogInformation("Google login added successfully for {Email}", user.Email);

            return user;
        }

        public async Task ChangePasswordAsync(string userId, ChangePasswordRequestDto model)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            if (model.NewPassword != model.ConfirmNewPassword)
                throw new ArgumentException("New password and confirm password misMatch.");
            if (model.OldPassword == model.NewPassword)
                throw new ArgumentException("New password cannot be the same as the old password.");
            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded)
                throw new InvalidOperationException("Password change failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            await _emailSender.SendEmailAsync(user.Email, "Password Changed", "Your password has been changed successfully.");
        }

        public async Task deleteUserAsync(string userId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
                throw new KeyNotFoundException("User not found");
            user.isDeleted = true;
            await _userRepository.UpdateUserAsync(user);
        }
    }
}
