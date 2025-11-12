using Dactra.DTOs;
using Dactra.Models;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dactra.Repositories;
using Dactra.Repositories.Implementation;
using Dactra.Repositories.Interfaces;
using Dactra.DTOs.AuthemticationDTOs;

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

        public AccountController(UserManager<ApplicationUser> usermanager, ITokenService tokenService, SignInManager<ApplicationUser> signInManager, IUserService userService, ApplicationDbContext context, IEmailSender emailSender, IUserRepository userRepository)
        {
            _userManager = usermanager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userService = userService;
            _context = context;
            _emailSender = emailSender;
            _userRepository = userRepository;
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

        [HttpGet("AllUsers..JustForTestingNow")]
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

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email.ToLower() == loginDto.Email.ToLower());
            if (user == null)
                return Unauthorized("invalid Email");
            if(!user.IsVerified)
                return BadRequest("not verified");


            var resulte = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if (!resulte.Succeeded)
                return Unauthorized(" Email or password Invalid");
            

            return Ok(
                    new NewUserDto
                    {
                        Email = user.Email,
                        Username = user.UserName,
                        Token = _tokenService.CreateToken(user),
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
            await _context.SaveChangesAsync();
            return Ok("OTP verified successfuly");
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
        [HttpDelete("deleteAllUsers..JustForTestingNow")]
        public async Task<IActionResult> DeleteAllUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            foreach (var user in users)
            {
                await _userManager.DeleteAsync(user);
            }
            return Ok("All users deleted.");
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

        [HttpDelete("DeleteUserByid")]
        public async Task<IActionResult> DeleteUserByID([FromHeader] string id)
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
        [HttpGet("GetUserById")]
        public async Task<IActionResult> GetUserByID([FromHeader] string id)
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
    }
}
