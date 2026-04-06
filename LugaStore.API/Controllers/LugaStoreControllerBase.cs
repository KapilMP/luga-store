using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using LugaStore.Application.Common.Settings;
using Microsoft.AspNetCore.RateLimiting;

namespace LugaStore.API.Controllers;

[ApiController]
[EnableRateLimiting(nameof(RateLimitingPolicies.Global))]
public abstract class LugaStoreControllerBase : ControllerBase
{
    protected int GetUserId()
    {
        var userIdStr = User.FindFirstValue(JwtRegisteredClaimNames.Sub);

        if (!int.TryParse(userIdStr, out var userId))
            throw new UnauthorizedAccessException();

        return userId;
    }
}
