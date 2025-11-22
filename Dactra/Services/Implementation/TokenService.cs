using Dactra.Models;
using Dactra.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Dactra.Services.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly ApplicationDbContext _context;
        public TokenService(IConfiguration config, ApplicationDbContext context)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SignInKey"]));
            _context = context;
        }
        public string CreateToken(ApplicationUser user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName,user.UserName),

            };
            var creds= new SigningCredentials(_key ,SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],

            };
            var tokenHandler =new  JwtSecurityTokenHandler();
            var token=tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public async Task<string> CreateRefreshToken(ApplicationUser user)
        {
            var refreshToken = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            var entity = new UserRefreshToken
            {
                Token = refreshToken,
                UserId = user.Id,
                ExpireAt = DateTime.UtcNow.AddDays(30), 
            };

             _context.UserRefreshTokens.Add(entity);
            await _context.SaveChangesAsync();

            return refreshToken;
        }
        public ClaimsPrincipal? ValidateAccessToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = _key,
                    ValidateIssuer = true,
                    ValidIssuer = _config["JWT:Issuer"],
                    ValidateAudience = true,
                    ValidAudience = _config["JWT:Audience"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                return handler.ValidateToken(token, parameters, out _);
            }
            catch
            {
                return null;
            }
        }
        public async Task<(string? AccessToken, string? Message)> RefreshAccessTokenAsync(string refreshToken)
        {
            var tokenEntity = await _context.UserRefreshTokens
                .OrderBy(o => o.ExpireAt)
                .LastOrDefaultAsync(t => t.Token == refreshToken && !t.IsUsed);

            if (tokenEntity == null)
                return (null, "Invalid Refresh Token");

            if (tokenEntity.ExpireAt < DateTime.UtcNow)
                return (null, "Refresh Token expired");

            var user = await _context.Users.FindAsync(tokenEntity.UserId);
            if (user == null)
                return (null, "User not found");

            
            tokenEntity.IsUsed = true;
            await _context.SaveChangesAsync();

          
            var newAccessToken = CreateToken(user);

            return (newAccessToken, "Token refreshed");
        }
    }

}


