using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Shops.Commands;

public record UpdateShopMemberActiveStatusCommand(int ShopId, int UserId, UserRole Role, bool IsActive) : IRequest;

public class UpdateShopMemberActiveStatusHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateShopMemberActiveStatusCommand>
{
    public async Task Handle(UpdateShopMemberActiveStatusCommand request, CancellationToken ct)
    {
        var member = await dbContext.ShopMembers
            .Include(sm => sm.User)
            .FirstOrDefaultAsync(sm => sm.ShopId == request.ShopId && sm.UserId == request.UserId && sm.User.Role == request.Role, ct)
            ?? throw new NotFoundException("User not found.");

        member.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(ct);
    }
}

