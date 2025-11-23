using Dactra.Models;
using Google.Apis.Auth;
using Dactra.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dactra.Repositories.Interfaces;
using Dactra.Services.Interfaces;


namespace Dactra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExternalAuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthCoreService _authCoreService;
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ExternalAuthController> _logger;
        private readonly IRoleRepository _roleRepository;

        public ExternalAuthController(
            UserManager<ApplicationUser> userManager,
            IAuthCoreService authCoreService,
            ApplicationDbContext context,
            IConfiguration configuration,
            ILogger<ExternalAuthController> logger,
            IRoleRepository userRepository)
        {
            _userManager = userManager;
            _authCoreService = authCoreService;
            _context = context;
            _configuration = configuration;
            _logger = logger;
            _roleRepository = userRepository;
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
        {
            _logger.LogInformation("GoogleLogin endpoint called.");

            if (request == null || string.IsNullOrEmpty(request.IdToken))
                return BadRequest(new { error = "Invalid request" });

            var payload = await GoogleTokenValidator.ValidateAsync(request.IdToken, _configuration, _logger);
            if (payload == null || string.IsNullOrEmpty(payload.Email))
                return BadRequest(new { error = "Invalid Google token" });

            var requestedRole = string.IsNullOrEmpty(request.Role) ? "Patient" : request.Role.Trim();
            var allowedRoles = new[] { "Patient", "Doctor" };
            if (!allowedRoles.Any(r => r.Equals(requestedRole, StringComparison.OrdinalIgnoreCase)))
                requestedRole = "Patient";

            var email = payload.Email!;
            var rawDeviceInfo = Request.Headers["User-Agent"].ToString() ?? "unknown";
            var deviceInfo = rawDeviceInfo.Length > 256 ? rawDeviceInfo[..256] : rawDeviceInfo;
            string ipAddress = Request.Headers.ContainsKey("X-Forwarded-For")
                ? Request.Headers["X-Forwarded-For"].ToString().Split(',').FirstOrDefault()?.Trim() ?? "unknown"
                : HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";

            ApplicationUser user = null!;
            using (var tx = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        user = new ApplicationUser
                        {
                            UserName = email,
                            Email = email,
                            EmailConfirmed = true,
                            CreatedAt = DateTime.UtcNow,
                            IsActive = false,
                            IsVerified = false,
                            IsRegistrationComplete = false
                            

                        };
                        var createResult = await _userManager.CreateAsync(user);
                        if (!createResult.Succeeded)
                            return BadRequest(new
                            {
                                success = false,
                                error = string.Join(", ", createResult.Errors.Select(e => e.Description))
                            });
                        await _roleRepository.AddUserToRoleAsync(user, requestedRole);

                        //if (requestedRole.Equals("Patient", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    if (!await _context.Patients.AnyAsync(p => p.PatientID == user.Id))
                        //    {
                        //        _context.Patients.Add(new HealthCare_.Models.PatientModels.Patient
                        //        {
                        //            PatientID = user.Id,
                        //            DateOfBirth = DateTime.Today.AddYears(-25),
                        //            Gender = "Unknown",
                        //            CreatedAt = DateTime.UtcNow,
                        //            CurrentLocation = "Not Specified"
                        //        });
                        //    }
                        //}
                        //else if (requestedRole.Equals("Doctor", StringComparison.OrdinalIgnoreCase))
                        //{
                        //    if (!await _context.Doctors.AnyAsync(d => d.DoctorID == user.Id))
                        //    {
                        //        _context.Doctors.Add(new HealthCare_.Models.DoctorModels.Doctor
                        //        {
                        //            DoctorID = user.Id,
                        //            Specialization = "General",
                        //            YearsOfExperience = 0,
                        //            ConsultationFee = 0,
                        //            IsActive = true,
                        //            CreatedAt = DateTime.UtcNow
                        //        });
                        //    }
                        //}

                        await _context.SaveChangesAsync();
                    }
                    //else
                    //{
                    //    if (!string.IsNullOrEmpty(request.Role) &&
                    //        !user.Role.Equals(requestedRole, StringComparison.OrdinalIgnoreCase))
                    //    {
                    //        await tx.RollbackAsync();
                    //        return BadRequest(new
                    //        {
                    //            success = false,
                    //            error = "Role mismatch for existing account. Please use the role upgrade process."
                    //        });
                    //    }
                    //}

                    await tx.CommitAsync();
                }
                catch (Exception ex)
                {
                    await tx.RollbackAsync();
                    _logger.LogError(ex, "Error during GoogleLogin user transaction for {Email}", email);
                    return StatusCode(500, new { success = false, error = "Server error" });
                }
            }

            var result = await _authCoreService.ExternalLoginAsync(user, deviceInfo, ipAddress);

            return Ok(new
            {
                success = true,
                accessToken = result.AccessToken,
                refreshToken = result.RefreshToken
            });
        }
    }

    public static class GoogleTokenValidator
    {
        public static async Task<GooglePayload?> ValidateAsync(string idToken, IConfiguration config, ILogger? logger = null)
        {
            try
            {
                var audiences = config.GetSection("Auth:GoogleClientIds").Get<List<string>>() ?? new List<string>();
                var settings = new GoogleJsonWebSignature.ValidationSettings { Audience = audiences };
                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);

                return new GooglePayload { Email = payload.Email, Name = payload.Name };
            }
            catch (Exception ex)
            {
                logger?.LogWarning(ex, "Google token validation failed");
                return null;
            }
        }
    }

    public class GooglePayload
    {
        public string Email { get; set; } = string.Empty;
        public string? Name { get; set; }
    }

    public class GoogleLoginRequest
    {
        public string IdToken { get; set; } = string.Empty;
        public string? Role { get; set; }
    }
}
