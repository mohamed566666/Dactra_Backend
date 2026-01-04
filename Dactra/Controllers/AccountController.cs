namespace Dactra.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserService _userService;
        private readonly ApplicationDbContext _context;
        private readonly IEmailSender _emailSender;
        private readonly IUserRepository _userRepository;
        private readonly IPasswordResetRepository _passwordResetRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly ILogger<AccountController> _logger;
        private readonly HttpContext _httpContext;


        public AccountController(UserManager<ApplicationUser> usermanager, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, IUserService userService, ApplicationDbContext context, IEmailSender emailSender, IUserRepository userRepository, IPasswordResetRepository passwordResetRepository, IRoleRepository roleRepository, ILogger<AccountController> logger)
        {
            _userManager = usermanager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userService = userService;
            _context = context;
            _emailSender = emailSender;
            _userRepository = userRepository;
            _passwordResetRepository = passwordResetRepository;
            _roleRepository = roleRepository;
            _logger = logger;
            
        }
        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            if (model == null)
                return BadRequest(new { Error = "Request body is required." });
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (model.Password != model.ConfirmPassword)
                return BadRequest("Password and Confirm Password do not match.");
            var result = await _userService.RegisterAsync(model);
            if (!result.Succeeded)
                return BadRequest(result.Errors);
            var Response = new RegisterResponseDto
            {
                UserId = result.Succeeded ? (await _userManager.FindByEmailAsync(model.Email)).Id : null,
                Email = model.Email
            };
            return Ok(Response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());
            if (user == null)
                return Unauthorized("invalid Email");

            if (user.isDeleted)
                return NotFound("User not found");

            var role= await _roleRepository.GetUserRolesAsync(user);
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized(" Email or password Invalid");
            if (role.Contains("Admin")) {

                var refreshToken1 = await _tokenService.CreateRefreshToken(user);

                Response.Cookies.Append("refreshToken", refreshToken1, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Path = "/",
                    Expires = DateTime.UtcNow.AddDays(30)
                });
                return Ok(new AdminDto
                {
                    Email = user.Email,
                    Username = user.UserName,
                    Token = _tokenService.CreateToken(user).ToString(),
                    massage= "Admin Login Successful"
                }
                );
            }
          

            if (!user.IsVerified)
                return BadRequest(new { massage=" not verified",Email=user.Email,role=role });
            if(!user.IsRegistrationComplete)
                return BadRequest(new { massage = "Registration not Completed", Email = user.Email, role = role } );
            var refreshToken = await _tokenService.CreateRefreshToken(user);
            Response.Cookies.Append("refreshToken", refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path ="/",
                Expires = DateTime.UtcNow.AddDays(30)
            }); 

            return Ok(
                    new NewUserDto
                    {
                        Email = user.Email,
                        Username = user.UserName,
                        Token = _tokenService.CreateToken(user).ToString(),

                        IsRegistrationComplete= user.IsRegistrationComplete,
                    }
            );

        }
        [HttpPost("verifyOTP")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyOtpDto VerifyDTO)
        {

            var otp = _context.EmailVerifications.Where(o => o.Email == VerifyDTO.Email).OrderBy(o=>o.ExpiryDate).LastOrDefault();
            if (otp == null)
                return BadRequest("NOT Found");

            if (DateTime.UtcNow > otp.ExpiryDate)
            {
                return BadRequest("OTP Expired");
            }
            bool valid = otp.OTP == VerifyDTO.OTP;
            if (!valid)
                return BadRequest("Invalid OTP");

            var user=await _userRepository.GetUserByEmailAsync(otp.Email);
            user.IsVerified = true;
            user.IsActive = true;
            user.EmailConfirmed = true;
            await _context.SaveChangesAsync();
            return Ok("OTP verified successfuly");
        }
        [HttpPost("verifyOTP_Forgetpassword")]
        public async Task<IActionResult> VerifyOtp_ForgetPassword([FromBody] VerifyOtpDto VerifyDTO)
        {

            var otp = _context.EmailVerifications.Where(o => o.Email == VerifyDTO.Email).OrderBy(o=>o.ExpiryDate).LastOrDefault();
            if (otp == null)
                return BadRequest("NOT Found");

            if (DateTime.UtcNow > otp.ExpiryDate)
            {
                return BadRequest("OTP Expired");
            }
            bool valid = otp.OTP == VerifyDTO.OTP;
            if (!valid)
                return BadRequest("Invalid OTP");

            var user=await _userRepository.GetUserByEmailAsync(otp.Email);
            user.IsVerified = true;
            user.IsActive = true;
            var refreshToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenEntity = new UserRefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpireAt = DateTime.UtcNow.AddDays(20),
                IsUsed = false
            };
            _context.UserRefreshTokens.Add(tokenEntity);
            await _context.SaveChangesAsync();
          
            return Ok(new {massage= "OTP verified successfuly", tokenEntity });
        }
        [HttpPost("resendOTP")]
        public async Task<IActionResult> ResendOTP([FromBody] ResendOTPDto resendOTPDto)
        {
            var otp = await _context.EmailVerifications.Where(o => o.Email == resendOTPDto.Email).FirstOrDefaultAsync();

            if (otp != null && DateTime.UtcNow < otp.ExpiryDate)
            {
                await _emailSender.SendEmailAsync(resendOTPDto.Email, "Verification Code To Dactra", $"Your OTP is: <b>{otp.OTP}</b>");
                return Ok("OTP resended");
            }
            string newCode = new Random().Next(100000, 999999).ToString();
            await _emailSender.SendEmailAsync(resendOTPDto.Email, "Verification Code To Dactra", $"Your new OTP is: <b>{newCode}</b>");
            var res = new EmailVerification
            {
                Email = resendOTPDto.Email,
                OTP = newCode,
                ExpiryDate = DateTime.UtcNow.AddMinutes(5),

            };
            _context.EmailVerifications.Add(res);
            await _context.SaveChangesAsync();
            return Ok("OTP sent successfully");
        }

        [HttpPost("SendOTP")]
        public async Task<IActionResult> TestEmail([FromBody] SendOTPtoMailDTO sendOtpDto)
        {
            var user = await _userManager.FindByEmailAsync(sendOtpDto.Email);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            await _userService.SendDTOforVerficatio(sendOtpDto);
            return Ok("OTP sent successfully.");
        }

        [AllowAnonymous]
        [HttpPost("reset-password")]
        public async Task<IActionResult> Resetpassword([FromBody] ResetPasswordTokenDto model)
        {
            var result = await _passwordResetRepository.ResetPasswordUsingRefreshTokenAsync(model);
            if (!result)
                return BadRequest("Invalid or expired token, or password mismatch.");

            return Ok("Password has been successfully reset.");
        }

        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated.");
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return BadRequest(new { success = false, message = "Validation failed", errors });
            }
            try
            {
                await _userService.ChangePasswordAsync(userId, model);
                return Ok("Password changed successfully.");
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var oldToken = Request.Cookies["refreshToken"];
            if (oldToken == null) return Unauthorized();


            var user = await _tokenService.GetUserByRefreshToken(oldToken);
            if (user == null) return Unauthorized();

            var access = _tokenService.CreateToken(user);
            var newRefresh = await _tokenService.CreateRefreshToken(user);


            Response.Cookies.Append("refreshToken", newRefresh, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = DateTime.UtcNow.AddDays(30)
            });

            return Ok(new { accessToken = access });
        }
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {

            var refreshToken = Request.Cookies["refreshToken"];
            if (!string.IsNullOrEmpty(refreshToken))
            {

                var user = await _tokenService.GetUserByRefreshToken(refreshToken);
                if (user != null)
                {
                    await _tokenService.RemoveRefreshToken(user);
                }
            }
            Response.Cookies.Delete("refreshToken", new CookieOptions
            {
                Path = "/",
                Secure = true,
                SameSite = SameSiteMode.None
            });
            return Ok(new { message = "Logged out successfully" });
        }

        [HttpGet("GetAllUsers")]
        public async Task<IActionResult> GetCurrentUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var ret = users.Select(u => new UserBasicsDTO
            {
                Id = u.Id,
                UserName = u.UserName,
                EmailConfirmed = u.EmailConfirmed,
                IsVerified = u.IsVerified,
            }).ToList();
            return Ok(ret);
        }

        [HttpGet("GetUserById/{id}")]
        public async Task<IActionResult> GetUserByID(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound("User not found.");
            }
            var userDto = new UserBasicsDTO
            {
                Id = user.Id,
                UserName = user.UserName,
                EmailConfirmed = user.EmailConfirmed,
                IsVerified = user.IsVerified,
            };
            return Ok(userDto);
        }

        [HttpGet("GetAllEmailsVerfications")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetAllEmailsVerfications()
        {
            var otps = await _context.EmailVerifications.ToListAsync();
            var ret = otps.Select(u => new VerficationsDTP
            {
                Id = u.Id,
                Email = u.Email,
                ExpiryDate = u.ExpiryDate,

            }).ToList();
            return Ok(ret);
        }

        [HttpGet("login/google")]
        public IActionResult GoogleLogin([FromQuery]string returnUrl , LinkGenerator linkGenerator, SignInManager<ApplicationUser> signInManager)
        {
            var properties = signInManager.ConfigureExternalAuthenticationProperties(provider: "Google", redirectUrl: linkGenerator.GetPathByName(HttpContext, endpointName: "GoogleResponse")
                + $"?returnUrl={returnUrl}");
            return Challenge(properties, authenticationSchemes:"Google");
        }

        [HttpGet("login/google/callback")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {
            // Get the info from the external login provider
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (!result.Succeeded)
            {
                return BadRequest(new { error = "External authentication error" });
            }

            var externalPrincipal = result.Principal;
            var email = externalPrincipal?.FindFirst(c => c.Type == ClaimTypes.Email)?.Value;
            var name = externalPrincipal?.FindFirst(c => c.Type == ClaimTypes.Name)?.Value;
            var providerKey = externalPrincipal?.FindFirst(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var provider = result.Properties?.Items[".AuthScheme"] ?? GoogleDefaults.AuthenticationScheme;

            if (string.IsNullOrEmpty(email))
                return BadRequest(new { error = "Email not provided by external provider" });

            // Try find user
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // create user
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    // set other fields if you have
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                    return StatusCode(500, new { error = "Failed to create local user", details = createResult.Errors });

                // you may want to add external login info too
            }

            // Sign in the user (local cookie)
            await _signInManager.SignInAsync(user, isPersistent: false);

            // create tokens
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.CreateToken(user);
            var refreshToken = _tokenService.CreateRefreshToken(user);
            var refreshExpires = DateTime.UtcNow.AddDays(30);

            // Save refresh token in DB
           

            // Set refresh token cookie
            Response.Cookies.Append("refreshToken",await refreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/",
                Expires = refreshExpires
            });

            // Option A: redirect to front-end with access token in query (less secure)
            // string redirectUrl = $"{returnUrl}?accessToken={accessToken}";

            // Option B: return JSON with access token (if callback is called by frontend)
         

            // Clear the external cookie
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            return Ok(accessToken);
        }
        [Authorize]
        [HttpDelete("DeleteMe")]
        public async Task<IActionResult> DeleteUserByID()
        {
           var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
           await _userService.deleteUserAsync(id);
           return Ok("User deleted successfully");
        }
    }
}
