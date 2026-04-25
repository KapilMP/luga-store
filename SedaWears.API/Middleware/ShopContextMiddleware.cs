using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.API.Middleware;

public class ShopContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser, IApplicationDbContext dbContext)
    {
        if ((currentUser.Role == UserRole.Manager || currentUser.Role == UserRole.Owner) &&
            currentUser.Id.HasValue && currentUser.ShopId.HasValue)
        {
            var hasAccess = await dbContext.ShopMembers
                .AnyAsync(sm => sm.UserId == currentUser.Id.Value &&
                               sm.ShopId == currentUser.ShopId.Value);

            if (!hasAccess)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                await context.Response.WriteAsJsonAsync(new
                {
                    Title = "Not Found",
                    Status = StatusCodes.Status404NotFound,
                    Detail = "Shop not found."
                });
                return;
            }
        }

        await next(context);
    }
}
