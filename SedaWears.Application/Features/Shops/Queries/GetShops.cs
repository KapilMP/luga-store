using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Shops.Models;
using SedaWears.Application.Common.Models;

namespace SedaWears.Application.Features.Shops.Queries;

public record GetShopsQuery(int PageNumber = 1, int PageSize = 10, bool? IsActive = null) : IRequest<PaginatedList<ShopRepresentation>>;

public class GetShopsHandler(IApplicationDbContext dbContext) : IRequestHandler<GetShopsQuery, PaginatedList<ShopRepresentation>>
{
    public async Task<PaginatedList<ShopRepresentation>> Handle(GetShopsQuery request, CancellationToken ct)
    {
        var query = dbContext.Shops
            .Include(s => s.Owners)
            .ThenInclude(o => o.Owner)
            .AsNoTracking();

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ShopRepresentation(
                s.Id,
                s.Name,
                s.Slug,
                s.Description,
                s.LogoFileName,
                s.IsActive,
                s.CreatedAt,
                s.Owners.Select(o => new ShopOwnerSummary(o.OwnerId, o.Owner.FirstName ?? string.Empty, o.Owner.LastName ?? string.Empty, o.Owner.Email ?? string.Empty)).ToList()
            ))
            .ToListAsync(ct);

        return new PaginatedList<ShopRepresentation>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
