using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Shops.Models;
using SedaWears.Application.Common.Models;

namespace SedaWears.Application.Features.Shops.Queries;

public record GetShopsQuery(
    int PageNumber = 1, 
    int PageSize = 10, 
    bool? IsActive = null,
    string? SortBy = null,
    string? SortOrder = "desc") : IRequest<PaginatedList<ShopRepresentation>>;

public class GetShopsHandler(IApplicationDbContext dbContext) : IRequestHandler<GetShopsQuery, PaginatedList<ShopRepresentation>>
{
    public async Task<PaginatedList<ShopRepresentation>> Handle(GetShopsQuery request, CancellationToken ct)
    {
        var query = dbContext.Shops
            .AsNoTracking();

        if (request.IsActive.HasValue)
            query = query.Where(s => s.IsActive == request.IsActive.Value);

        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var isDescending = request.SortOrder?.ToLower() == "desc";
            query = request.SortBy.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
                "slug" => isDescending ? query.OrderByDescending(s => s.Slug) : query.OrderBy(s => s.Slug),
                _ => isDescending ? query.OrderByDescending(s => s.Id) : query.OrderBy(s => s.Id)
            };
        }
        else
        {
            query = query.OrderByDescending(s => s.Id);
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(s => new ShopRepresentation(
                s.Id,
                s.Name,
                s.Slug,
                s.Description,
                s.LogoFileName,
                s.IsActive
            ))
            .ToListAsync(ct);

        return new PaginatedList<ShopRepresentation>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
