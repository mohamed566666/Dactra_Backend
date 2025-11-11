using Dactra.DTOs;
using Dactra.Models;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public AccountController(UserManager<ApplicationUser> usermanager,ITokenService tokenService, SignInManager <ApplicationUser> signInManager , IUserService userService)
        {
            _userManager = usermanager;
            _tokenService = tokenService;
            _signInManager = signInManager;
            _userService = userService;
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

        [HttpGet("CurrentUsers..JustForTestingNow")]
        public async Task<IActionResult> GetCurrentUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            var ret = users.Select(u => new UserBasicsDTO
            {
                UserName = u.UserName,
                EmailConfirmed = u.EmailConfirmed,
                IsVerified = u.IsVerified,
            }).ToList();
            return Ok(ret);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]LoginDto loginDto  )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user= await _userManager.Users.FirstOrDefaultAsync(u=> u.Email.ToLower() == loginDto.Email.ToLower());
            if(user==null)
                return Unauthorized("invalid Email");


            var resulte = await _signInManager.CheckPasswordSignInAsync(user, loginDto.Password, false);

            if(!resulte.Succeeded) 
                return Unauthorized(" Email or password Invalid");

            return Ok(
                    new NewUserDto
                    {
                        Email = user.Email,
                        Username=user.UserName,
                        Token=_tokenService.CreateToken(user),
                    }
                
                );


        }



    }


}
