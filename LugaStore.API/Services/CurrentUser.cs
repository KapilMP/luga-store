using System.Security.Claims;
using LugaStore.Application.Common.Interfaces;
using System.IdentityModel.Tokens.Jwt;

namespace LugaStore.API.Services;

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
}
