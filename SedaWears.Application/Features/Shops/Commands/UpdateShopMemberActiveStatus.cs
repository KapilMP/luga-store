using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Shops.Commands;

public record UpdateShopMemberActiveStatusCommand(int ShopId, int UserId, bool IsActive) : IRequest;

public class UpdateShopMemberActiveStatusHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateShopMemberActiveStatusCommand>
{
    public async Task Handle(UpdateShopMemberActiveStatusCommand request, CancellationToken ct)
    {
        var member = await dbContext.ShopMembers
            .FirstOrDefaultAsync(sm => sm.ShopId == request.ShopId && sm.UserId == request.UserId, ct)
            ?? throw new NotFoundException("Shop member not found.");

        member.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(ct);
    }
}
