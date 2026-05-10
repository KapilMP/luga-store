using SedaWears.Application.Common.Interfaces;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SedaWears.Domain.Enums;

namespace SedaWears.API.Middleware;

public class UserStatusMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, IApplicationDbContext dbContext)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var sub = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
            var roleClaim = context.User.FindFirstValue(ClaimTypes.Role);

            if (int.TryParse(sub, out var userId) && Enum.TryParse<UserRole>(roleClaim, out var role))
            {
                var userValid = await dbContext.Users
                    .AnyAsync(u => u.Id == userId && u.Role == role && u.IsActive);

                if (!userValid)
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsJsonAsync(new { message = "User not found, inactive, or role has changed." });
                    return;
                }
            }
        }

        await next(context);
    }
}

public static class UserStatusMiddlewareExtensions
{
    public static IApplicationBuilder UseUserStatusValidation(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UserStatusMiddleware>();
    }
}
