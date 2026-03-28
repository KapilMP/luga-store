using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Products.Queries;

public record GetProductsQuery(
    Gender? Gender = null,
    int? CategoryId = null,
    string? CategorySlug = null,
    string? SortBy = null,
    int PageNumber = 1,
    int PageSize = 10
) : IRequest<PaginatedResult<ProductBrowseDto>>;

public class GetProductsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetProductsQuery, PaginatedResult<ProductBrowseDto>>
{
    public async Task<PaginatedResult<ProductBrowseDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var query = context.Products
            .AsNoTracking()
            .AsQueryable();

        // Filtering
        if (request.Gender.HasValue)
            query = query.Where(p => p.Gender == request.Gender.Value);

        if (request.CategoryId.HasValue)
            query = query.Where(p => p.Categories.Any(c => c.Id == request.CategoryId.Value));

        if (!string.IsNullOrEmpty(request.CategorySlug))
            query = query.Where(p => p.Categories.Any(c => c.Slug == request.CategorySlug));

        // Sorting
        query = request.SortBy switch
        {
            "price_asc" => query.OrderBy(p => p.Price),
            "price_desc" => query.OrderByDescending(p => p.Price),
            "newest" => query.OrderByDescending(p => p.Id),
            _ => query.OrderByDescending(p => p.Id)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Include(p => p.Categories)
                .ThenInclude(c => c.Partner)
            .Include(p => p.SizeStocks)
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductBrowseDto(
                p.Id,
                p.Name,
                p.Price,
                p.Description,
                p.Categories.Any(c => c.PartnerId != null),
                p.Categories.Any(c => c.Partner != null)
                    ? $"{p.Categories.First(c => c.Partner != null).Partner!.FirstName} {p.Categories.First(c => c.Partner != null).Partner!.LastName}"
                    : "Luga Brand",
                p.SizeStocks.Select(s => new ProductSizeStockResponseDto(s.Size, s.Stock))
            ))
            .ToListAsync(cancellationToken);

        return items.ToPaginatedResult(totalCount, request.PageNumber, request.PageSize);
    }
}
