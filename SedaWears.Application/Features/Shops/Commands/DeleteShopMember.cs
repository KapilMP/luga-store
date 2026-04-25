using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Shops.Commands;

public record DeleteShopMemberCommand(int ShopId, int UserId) : IRequest;

public class DeleteShopMemberHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteShopMemberCommand>
{
    public async Task Handle(DeleteShopMemberCommand request, CancellationToken ct)
    {
        var member = await dbContext.ShopMembers
            .Include(sm => sm.User)
            .FirstOrDefaultAsync(sm => sm.ShopId == request.ShopId && sm.UserId == request.UserId, ct) 
            ?? throw new NotFoundException("Shop member not found.");

        dbContext.ShopMembers.Remove(member);

        // Check if the user is associated with any OTHER shops
        var hasOtherMemberships = await dbContext.ShopMembers
            .AnyAsync(sm => sm.UserId == request.UserId && sm.ShopId != request.ShopId, ct);

        if (!hasOtherMemberships)
        {
            var user = member.User;
            // Only auto-delete the user if their primary role is shop-related (Manager/Owner)
            // This prevents accidental deletion of Admins or Customers who have shop memberships
            if (user.Role == UserRole.Manager || user.Role == UserRole.Owner)
            {
                dbContext.Users.Remove(user);
            }
        }

        await dbContext.SaveChangesAsync(ct);
    }
}
