using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Common.Settings;
using LugaStore.Application.Features.UserManagement.Models;

namespace LugaStore.Application.Features.UserManagement.Queries;

public record GetPartnerManagersQuery(int PartnerId, int PageNumber = 1, int PageSize = 10, bool? Invited = null, bool? IsActive = null) : IRequest<PaginatedList<PartnerManagerRepresentation>>;

public class GetPartnerManagersQueryHandler(IApplicationDbContext dbContext) : 
    IRequestHandler<GetPartnerManagersQuery, PaginatedList<PartnerManagerRepresentation>>
{
    public async Task<PaginatedList<PartnerManagerRepresentation>> Handle(GetPartnerManagersQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.PartnerManagers
            .AsNoTracking()
            .Where(pm => pm.PartnerId == request.PartnerId)
            .Select(pm => pm.Manager);

        if (request.Invited.HasValue)
        {
            query = query.Where(u => u.EmailConfirmed != request.Invited.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<PartnerManagerRepresentation>(
            items.Select(u => u.ToPartnerManagerRepresentation()).ToList(),
            count,
            request.PageNumber,
            request.PageSize);
    }
}
