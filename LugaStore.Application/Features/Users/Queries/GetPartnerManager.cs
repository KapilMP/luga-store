using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Features.Users.Models;

namespace LugaStore.Application.Features.Users.Queries;

public record GetPartnerManagerQuery(int PartnerId, int ManagerId) : IRequest<PartnerManagerRepresentation>;

public class GetPartnerManagerHandler(IApplicationDbContext dbContext) : IRequestHandler<GetPartnerManagerQuery, PartnerManagerRepresentation>
{
    public async Task<PartnerManagerRepresentation> Handle(GetPartnerManagerQuery request, CancellationToken ct)
    {
        var pm = await dbContext.PartnerManagers
            .AsNoTracking()
            .Include(x => x.Manager)
            .FirstOrDefaultAsync(x => x.PartnerId == request.PartnerId && x.ManagerId == request.ManagerId, ct)
            ?? throw new NotFoundError("Partner Manager not found");

        return PartnerManagerRepresentation.ToPartnerManagerRepresentation(pm.Manager);
    }
}
