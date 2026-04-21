using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;

namespace SedaWears.Application.Features.Users.Queries;

public record GetManagerQuery(int ManagerId) : IRequest<ManagerRepresentation>;

public class GetManagerHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetManagerQuery, ManagerRepresentation>
{
    public async Task<ManagerRepresentation> Handle(GetManagerQuery request, CancellationToken ct)
    {
        var shopId = currentUser.ShopId ?? throw new UnauthorizedAccessException("Shop context missing. Use X-Shop-ID.");

        var sm = await dbContext.ShopManagers
            .AsNoTracking()
            .Include(x => x.Manager)
            .ThenInclude(m => m.ManagedShops)
            .ThenInclude(ms => ms.Shop)
            .FirstOrDefaultAsync(x => x.ShopId == shopId && x.ManagerId == request.ManagerId, ct)
            ?? throw new NotFoundException("Shop Manager linkage not found");

        return (ManagerRepresentation)sm.Manager.ToUserRepresentation();
    }
}
