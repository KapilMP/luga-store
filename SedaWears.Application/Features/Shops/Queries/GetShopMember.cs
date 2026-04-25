using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Application.Features.Users;

namespace SedaWears.Application.Features.Shops.Queries;

public record GetShopMemberQuery(int ShopId, int UserId) : IRequest<BaseUserRepresentation>;

public class GetShopMemberHandler(IApplicationDbContext dbContext) : IRequestHandler<GetShopMemberQuery, BaseUserRepresentation>
{
    public async Task<BaseUserRepresentation> Handle(GetShopMemberQuery request, CancellationToken ct)
    {
        var member = await dbContext.ShopMembers
            .AsNoTracking()
            .Include(sm => sm.User)
            .ThenInclude(u => u.ShopMemberships)
            .ThenInclude(sm => sm.Shop)
            .FirstOrDefaultAsync(sm => sm.ShopId == request.ShopId && sm.UserId == request.UserId, ct) 
            ?? throw new NotFoundException("Shop member not found.");

        return member.User.ToUserRepresentation();
    }
}
