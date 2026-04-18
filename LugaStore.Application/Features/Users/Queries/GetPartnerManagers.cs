using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Features.Users.Models;

namespace LugaStore.Application.Features.Users.Queries;

public record GetPartnerManagersQuery(int PageNumber, int PageSize, bool? Invited, bool? IsActive) : IRequest<PaginatedList<PartnerManagerRepresentation>>;

public class GetPartnerManagersHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetPartnerManagersQuery, PaginatedList<PartnerManagerRepresentation>>
{
    public async Task<PaginatedList<PartnerManagerRepresentation>> Handle(GetPartnerManagersQuery request, CancellationToken ct)
    {
        var partnerId = currentUser.Id!.Value;

        var query = dbContext.PartnerManagers
            .AsNoTracking()
            .Where(pm => pm.PartnerId == partnerId)
            .Include(pm => pm.Manager)
            .OrderByDescending(pm => pm.Created)
            .AsQueryable();

        if (request.IsActive.HasValue)
            query = query.Where(pm => pm.Manager.IsActive == request.IsActive.Value);

        // Map then Paginate
        var mappedQuery = query.Select(pm => PartnerManagerRepresentation.ToPartnerManagerRepresentation(pm.Manager));
        
        return await mappedQuery.PaginatedListAsync(request.PageNumber, request.PageSize, ct);
    }
}
