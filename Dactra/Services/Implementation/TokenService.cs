namespace Dactra.Services.Implementation
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly ApplicationDbContext _context;
        private readonly IRoleRepository roleRepository;
        public TokenService(IConfiguration config, ApplicationDbContext context, IRoleRepository roleRepository)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SignInKey"]));
            _context = context;
            this.roleRepository = roleRepository;
        }
        public  string CreateToken(ApplicationUser user)
        {
            var userRoles = roleRepository.GetUserRolesAsync(user).Result;
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(JwtRegisteredClaimNames.GivenName,user.UserName),
                new Claim(ClaimTypes.Role,string.Join(",",userRoles)),

            };
            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(15),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"],

            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
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

            return (newAccessToken.ToString(), "Token refreshed");
        }

        public async Task<ApplicationUser?> GetUserByRefreshToken(string refreshToken)
        {
            
            var tokenEntity = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken);

            if (tokenEntity == null)
                return null;

            
            if (tokenEntity.ExpireAt < DateTime.UtcNow)
                return null;

           
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == tokenEntity.UserId);

            return user;
        }

        public async Task RemoveRefreshToken(ApplicationUser user)
        {
            var tokens = await _context.UserRefreshTokens
             .Where(t => t.UserId == user.Id)
              .ToListAsync();

            if (tokens.Any())
            {
                _context.UserRefreshTokens.RemoveRange(tokens);
                await _context.SaveChangesAsync();
            }
        }
    }

}


