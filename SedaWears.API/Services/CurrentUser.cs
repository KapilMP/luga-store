using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Services;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    public int? Id
    {
        get
        {
            var sub = httpContextAccessor.HttpContext?.User?.FindFirstValue(JwtRegisteredClaimNames.Sub);
            return int.TryParse(sub, out var id) ? id : null;
        }
    }

    public int? ShopId
    {
        get
        {
            var user = httpContextAccessor.HttpContext?.User;
            if (user == null) return null;

            var role = user.FindFirstValue(ClaimTypes.Role);

            if (role == nameof(UserRole.Owner) || role == nameof(UserRole.Manager))
            {
                var headerValue = httpContextAccessor.HttpContext?.Request.Headers["X-Shop-ID"].ToString();
                return int.TryParse(headerValue, out var shopId) ? shopId : null;
            }

            return null;
        }
    }

    public UserRole? Role
    {
        get
        {
            var role = httpContextAccessor.HttpContext?.User?.FindFirstValue("role");
            return Enum.TryParse<UserRole>(role, out var result) ? result : null;
        }
    }
}
