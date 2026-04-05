using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace LugaStore.API.Controllers;

[ApiController]
public abstract class LugaStoreControllerBase : ControllerBase
{
    protected int GetUserId()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? User.FindFirstValue("sub");

        if (!int.TryParse(userIdStr, out var userId))
            throw new UnauthorizedAccessException();

        return userId;
    }
}
