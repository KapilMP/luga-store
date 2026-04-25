using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Shops.Commands;

public record UpdateShopMemberCommand(int ShopId, int UserId, string FirstName, string LastName, bool IsActive) : IRequest;

public class UpdateShopMemberHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateShopMemberCommand>
{
    public async Task Handle(UpdateShopMemberCommand request, CancellationToken ct)
    {
        var member = await dbContext.ShopMembers
            .Include(sm => sm.User)
            .FirstOrDefaultAsync(sm => sm.ShopId == request.ShopId && sm.UserId == request.UserId, ct) 
            ?? throw new NotFoundException("Shop member not found.");

        member.User.FirstName = request.FirstName;
        member.User.LastName = request.LastName;
        member.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(ct);
    }
}
