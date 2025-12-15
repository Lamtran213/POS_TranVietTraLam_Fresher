using POS_TranVietTraLam_Fresher_Entities.Entity;
using System.Security.Claims;

namespace POS_TranVietTraLam_Fresher_BLL.Defines
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken(User user);
        ClaimsPrincipal? ValidateRefreshToken(string refreshToken);
        string? GetUserIdFromRefreshToken(string refreshToken);
    }
}
