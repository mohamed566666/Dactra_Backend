//using Dactra.Models;
//using Dactra.Services.Interfaces;
//using Dactra.Models;
//using Dactra.Services;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.Logging;
//using System;
//using System.Linq;
//using System.Threading.Tasks;




//namespace Dactra.Services.Implementation

//{
//    public class AuthCoreService : IAuthCoreService
//    {
//        private readonly ApplicationDbContext _context;
//        private readonly ITokenService _tokenService;
//        private readonly IConfiguration _configuration;
//        private readonly ILogger<AuthCoreService> _logger;

//        public AuthCoreService(
//            ApplicationDbContext context,
//            ITokenService tokenService,
//            IConfiguration configuration,
//            ILogger<AuthCoreService> logger)
//        {
//            _context = context;
//            _tokenService = tokenService;
//            _configuration = configuration;
//            _logger = logger;
//        }

//        public async Task<(string AccessToken, string RefreshToken, string? Error)> ExternalLoginAsync(
//            ApplicationUser user,
//            string? deviceInfo = null,
//            string? ipAddress = null)
//        {
//            if (user == null)
//            {
//                _logger.LogWarning("ExternalLoginAsync: User is null");
//                return (null!, null!, "User not found");
//            }

//            _logger.LogInformation("ExternalLoginAsync: Starting login for UserId={UserId}, Device={Device}, IP={IP}",
//                user.Id, deviceInfo, ipAddress);

//            try
//            {
//                // Generate access & refresh tokens
//                var token =  _tokenService.CreateToken(user);
//                var rawRefresh = _tokenService.CreateRefreshToken(user);

//                // Save refresh token in DB
//                var refreshEntity = new UserRefreshToken
//                {
//                    Token =await rawRefresh,
//                    ExpireAt = DateTime.UtcNow.AddDays(Convert.ToDouble(_configuration["Jwt:RefreshTokenExpireDays"] ?? "30")),
//                    UserId = user.Id,
//                };

//                _context.UserRefreshTokens.Add(refreshEntity);
//                await _context.SaveChangesAsync();

//                _logger.LogInformation("ExternalLoginAsync: Generated tokens for UserId={UserId}", user.Id);

//                return (token,await rawRefresh, string.Empty);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "ExternalLoginAsync: Failed for UserId={UserId}", user.Id);
//                return (null!, null!, "Server error");
//            }
//        }
//    }
//}
