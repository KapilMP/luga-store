using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.WebAPI.Identity;

public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
{
    private ClaimsPrincipal? User => httpContextAccessor.HttpContext?.User;

    public string? UserId => User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public bool IsAuthenticated => User?.Identity?.IsAuthenticated ?? false;
}
