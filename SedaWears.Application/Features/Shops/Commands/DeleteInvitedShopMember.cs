using MediatR;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Shops.Commands;

public record DeleteInvitedShopMemberCommand(int ShopId, int UserId, UserRole Role) : IRequest;

public class DeleteInvitedShopMemberHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteInvitedShopMemberCommand>
{
    public async Task Handle(DeleteInvitedShopMemberCommand request, CancellationToken ct)
    {
        var member = await dbContext.ShopMembers
            .Include(sm => sm.User)
            .FirstOrDefaultAsync(sm => sm.ShopId == request.ShopId && sm.UserId == request.UserId && sm.User.Role == request.Role && !sm.IsInvitationAccepted, ct)
            ?? throw new NotFoundException("User not found.");

        dbContext.ShopMembers.Remove(member);

        // Also check if we should delete the user
        var hasOtherMemberships = await dbContext.ShopMembers
            .AnyAsync(sm => sm.UserId == request.UserId && sm.ShopId != request.ShopId, ct);

        if (!hasOtherMemberships)
        {
            // If they haven't accepted ANY invitation and have no other memberships, we can delete the user
            // However, for safety, we only delete if they are exclusively a shop-role user
            if (member.User.Role == UserRole.Owner || member.User.Role == UserRole.Manager)
            {
                dbContext.Users.Remove(member.User);
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
