using Microsoft.AspNetCore.Http;
using POS_TranVietTraLam_Fresher_BLL.Defines;
using System.Security.Claims;

public class AuthenticatedUser : IAuthenticatedUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public AuthenticatedUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid UserId
    {
        get
        {
            if (User?.Identity?.IsAuthenticated != true)
                throw new UnauthorizedAccessException("User is not authenticated");

            var id = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(id, out var userId))
                throw new UnauthorizedAccessException("Invalid user id");

            return userId;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);

    public IEnumerable<string> Roles =>
        User?.FindAll(ClaimTypes.Role).Select(x => x.Value)
        ?? Enumerable.Empty<string>();
}
