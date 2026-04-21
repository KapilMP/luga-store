using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.API.Middleware;

public class ShopContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, ICurrentUser currentUser, IApplicationDbContext dbContext)
    {
        if (currentUser.Role == UserRole.Manager && currentUser.Id.HasValue && currentUser.ShopId.HasValue)
        {
            var hasAccess = await dbContext.ShopManagers
                .AnyAsync(sm => sm.ManagerId == currentUser.Id.Value &&
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
        else if (currentUser.Role == UserRole.Owner && currentUser.Id.HasValue && currentUser.ShopId.HasValue)
        {
            var hasAccess = await dbContext.ShopOwners
                .AnyAsync(so => so.OwnerId == currentUser.Id.Value &&
                               so.ShopId == currentUser.ShopId.Value);

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
