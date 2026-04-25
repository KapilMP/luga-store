using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Shops.Models;
using SedaWears.Application.Common.Models;

namespace SedaWears.Application.Features.Shops.Queries;

public record GetShopsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = "createdAt",
    string? SortOrder = "desc",
    string? Search = null) : IRequest<PaginatedList<ShopRepresentation>>;

public class GetShopsHandler(IApplicationDbContext dbContext) : IRequestHandler<GetShopsQuery, PaginatedList<ShopRepresentation>>
{
    public async Task<PaginatedList<ShopRepresentation>> Handle(GetShopsQuery request, CancellationToken ct)
    {
        var query = dbContext.Shops
            .IgnoreQueryFilters()
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim();
            query = query.Where(s => s.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                     s.Slug.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(request.SortBy))
        {
            var isDescending = request.SortOrder?.ToLower() == "desc";
            query = request.SortBy.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
                "slug" => isDescending ? query.OrderByDescending(s => s.Slug) : query.OrderBy(s => s.Slug),
                "isactive" => isDescending ? query.OrderByDescending(s => s.IsActive) : query.OrderBy(s => s.IsActive),
                "isdeleted" => isDescending ? query.OrderByDescending(s => s.IsDeleted) : query.OrderBy(s => s.IsDeleted),
                "createdat" => isDescending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt),
                _ => isDescending ? query.OrderByDescending(s => s.CreatedAt) : query.OrderBy(s => s.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(s => s.CreatedAt);
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
                s.BannerFileName,
                s.IsActive,
                s.IsDeleted,
                s.CreatedAt
            ))
            .ToListAsync(ct);

        return new PaginatedList<ShopRepresentation>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
