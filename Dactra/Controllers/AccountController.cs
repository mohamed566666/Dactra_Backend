using Dactra.DTOs;
using Dactra.DTOs.AccountDTOs;
using Dactra.DTOs.AuthemticationDTOs;
using Dactra.Models;
using Dactra.Repositories;
using Dactra.Repositories.Implementation;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Implementation;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using static System.Net.WebRequestMethods;

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
            var role= await _roleRepository.GetUserRolesAsync(user);

          
            if(!user.IsVerified)
                return BadRequest(new { massage=" not verified",Email=user.Email,role=role });
            if(!user.IsRegistrationComplete)
                return BadRequest(new { massage = "Registration not Completed", Email = user.Email, role = role } );
            


            var result = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!result.Succeeded)
                return Unauthorized(" Email or password Invalid");

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
        public IActionResult GoogleLogin(string returnUrl = "/")
        {
            var properties = new AuthenticationProperties { RedirectUri = Url.Action("GoogleResponse", new { returnUrl }) };
            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("login/google/callback")]
        public async Task<IActionResult> GoogleResponse(string returnUrl = "/")
        {

            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            if (!result.Succeeded)
            {
                return BadRequest("Google authentication failed");
            }


            var claims = result.Principal.Identities.FirstOrDefault()?.Claims;
            var email = claims?.FirstOrDefault(c => c.Type == "email")?.Value;
            var name = claims?.FirstOrDefault(c => c.Type == "name")?.Value;

            return Redirect(returnUrl);
        } 
        [HttpDelete("DeleteUserByid/{id}")]
        public async Task<IActionResult> DeleteUserByID(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound(new { message = "User not found" });
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Any())
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
                if (!removeResult.Succeeded)
                    return BadRequest(removeResult.Errors);
            }
            var result = await _userManager.DeleteAsync(user);
            if (result.Succeeded)
                return Ok(new { message = "User deleted successfully" });
            return BadRequest(result.Errors);
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

    }
}
