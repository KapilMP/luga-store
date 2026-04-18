using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Features.Users.Models;

namespace LugaStore.Application.Features.Users.Queries;

public record GetPartnerManagerQuery(int ManagerId) : IRequest<PartnerManagerRepresentation>;

public class GetPartnerManagerHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetPartnerManagerQuery, PartnerManagerRepresentation>
{
    public async Task<PartnerManagerRepresentation> Handle(GetPartnerManagerQuery request, CancellationToken ct)
    {
        var partnerId = currentUser.Id!.Value;
        var pm = await dbContext.PartnerManagers
            .AsNoTracking()
            .Include(pm => pm.Manager)
            .FirstOrDefaultAsync(x => x.PartnerId == partnerId && x.ManagerId == request.ManagerId, ct)
            ?? throw new NotFoundError("Partner Manager not found");

        return PartnerManagerRepresentation.ToPartnerManagerRepresentation(pm.Manager);
    }
}
