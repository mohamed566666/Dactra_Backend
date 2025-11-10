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
        public AccountController(UserManager<ApplicationUser> usermanager,ITokenService tokenService, SignInManager <ApplicationUser> signInManager)
        {
            _userManager = usermanager;
            _tokenService = tokenService;
            _signInManager = signInManager;
        }
        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        //{
        //    try {
        //             var user = new ApplicationUser
        //            {
        //                UserName = registerDto.name
        //            };
        //            var createuser= await _userManager.CreateAsync(user, registerDto.password);
        //            if (createuser.Succeeded) {
        //                var roleresult = await _userManager.AddToRoleAsync(user, "DoctorProfile");
        //                if (roleresult.Succeeded)
        //                {
        //                    return Ok(roleresult);
        //                }
        //                else
        //                {
        //                    return BadRequest();
        //                }

        //            }
        //            else { 
        //                return BadRequest(); 
        //            }

        //    }catch (Exception ex)
        //    {
        //        return BadRequest();
        //    }
        //}
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
