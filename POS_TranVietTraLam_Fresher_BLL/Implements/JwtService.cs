using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using POS_TranVietTraLam_Fresher_Entities.Entity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace POS_TranVietTraLam_Fresher_BLL.Implements
{
    public class JwtService : IJwtService
    {
        private readonly string _accessSecret;
        private readonly string _refreshSecret;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly ILogger<JwtService> _logger;

        public JwtService(IConfiguration configuration, ILogger<JwtService> logger)
        {
            _accessSecret = configuration["Jwt:AccessSecretKey"]!;
            _refreshSecret = configuration["Jwt:RefreshSecretKey"]!;
            _issuer = configuration["Jwt:Issuer"]!;
            _audience = configuration["Jwt:Audience"]!;
            _logger = logger;
        }

        public string GenerateAccessToken(User user, int expirationMinutes = 1440)
        {
            _logger.LogInformation("Generating JWT for user {UserId}", user.UserId);
            return GenerateToken(user, _accessSecret, expirationMinutes);
        }

        public string GenerateRefreshToken(User user, int expirationMinutes = 10080)
        {
            _logger.LogInformation("Generating Refresh JWT for user {UserId}", user.UserId);
            return GenerateToken(user, _refreshSecret, expirationMinutes);
        }

        private string GenerateToken(User user, string secretKey, int expirationMinutes)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _issuer,
                audience: _audience,
                claims: claims,
                notBefore: DateTime.UtcNow.AddSeconds(-5),
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: creds
            );

            _logger.LogInformation("JWT generated successfully, expires in {Minutes} minutes", expirationMinutes);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateRefreshToken(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                return null;

            try
            {
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_refreshSecret)); 
                var tokenHandler = new JwtSecurityTokenHandler();

                var principal = tokenHandler.ValidateToken(refreshToken, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }, out _);

                return principal;
            }
            catch (SecurityTokenExpiredException ex)
            {
                _logger.LogWarning("Refresh token expired: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid refresh token");
                return null;
            }
        }

    }
}
