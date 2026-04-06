using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Features.UserManagement.Models;

namespace LugaStore.Application.Features.UserManagement.Queries;

public record GetPartnerManagerQuery(int PartnerId, int ManagerId) : IRequest<PartnerManagerRepresentation>;

public class GetPartnerManagerQueryHandler(IApplicationDbContext dbContext) : 
    IRequestHandler<GetPartnerManagerQuery, PartnerManagerRepresentation>
{
    public async Task<PartnerManagerRepresentation> Handle(GetPartnerManagerQuery request, CancellationToken cancellationToken)
    {
        var mapping = await dbContext.PartnerManagers
            .AsNoTracking()
            .Include(pm => pm.Manager)
            .FirstOrDefaultAsync(pm => pm.PartnerId == request.PartnerId && pm.ManagerId == request.ManagerId, cancellationToken) 
            ?? throw new NotFoundError("Manager assignment not found.");

        return mapping.Manager.ToPartnerManagerRepresentation();
    }
}
