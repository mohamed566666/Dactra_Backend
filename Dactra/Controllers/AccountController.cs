using Dactra.DTOs;
using Dactra.Interface;
using Dactra.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dactra.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITokenService _tokenService;
        public AccountController(UserManager<ApplicationUser> usermanager,ITokenService tokenService)
        {
            _userManager = usermanager;
            _tokenService = tokenService;
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
    
    }


}
