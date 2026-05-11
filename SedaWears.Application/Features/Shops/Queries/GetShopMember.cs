using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Application.Features.Users.Projections;

namespace SedaWears.Application.Features.Shops.Queries;

public record GetShopMemberQuery(int ShopId, int UserId) : IRequest<BaseUserDto>;

public class GetShopMemberHandler(IApplicationDbContext dbContext) : IRequestHandler<GetShopMemberQuery, BaseUserDto>
{
    public async Task<BaseUserDto> Handle(GetShopMemberQuery request, CancellationToken ct)
    {
        var member = await dbContext.ShopMembers
            .AsNoTracking()
            .Include(sm => sm.User)
            .FirstOrDefaultAsync(sm => sm.ShopId == request.ShopId && sm.UserId == request.UserId, ct) 
            ?? throw new NotFoundException("Shop member not found.");

        return member.User.ToUserDto(member.CreatedAt);
    }
}
