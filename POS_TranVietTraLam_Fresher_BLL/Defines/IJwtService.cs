using POS_TranVietTraLam_Fresher_Entities.Entity;
using System.Security.Claims;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, int expirationMinutes = 1440);
        string GenerateRefreshToken(User user, int expirationMinutes = 10080);
        ClaimsPrincipal? ValidateRefreshToken(string refreshToken);
    }
}
