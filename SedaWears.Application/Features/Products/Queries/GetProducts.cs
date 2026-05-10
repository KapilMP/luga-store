using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Products.Models;
using SedaWears.Application.Features.Products.Projections;

using SedaWears.Application.Common.Validators;

namespace SedaWears.Application.Features.Products.Queries;

public record GetProductsQuery(
    int? CategoryId = null,
    int? ShopId = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortOrder = "asc",
    string? Search = null) : IRequest<PaginatedList<ProductRepresentation>>, IPaginatedQuery;

public class GetProductsValidator : PaginatedQueryValidator<GetProductsQuery> { }

public class GetProductsHandler(IApplicationDbContext dbContext) : IRequestHandler<GetProductsQuery, PaginatedList<ProductRepresentation>>
{
    public async Task<PaginatedList<ProductRepresentation>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var query = dbContext.Products
            .AsNoTracking();

        if (request.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == request.CategoryId.Value);
        }

        query = query.Where(p => p.Category.ShopId == request.ShopId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim();
            query = query.Where(p => EF.Functions.ILike(p.Name, $"%{searchTerm}%"));
        }

        var isDescending = request.SortOrder?.ToLower() == "desc";
        query = (request.SortBy?.ToLower()) switch
        {
            "name" => isDescending ? query.OrderByDescending(p => p.Name) : query.OrderBy(p => p.Name),
            "price" => isDescending ? query.OrderByDescending(p => p.Price) : query.OrderBy(p => p.Price),
            "createdat" => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt),
            _ => isDescending ? query.OrderByDescending(p => p.CreatedAt) : query.OrderBy(p => p.CreatedAt)
        };

        var totalCount = await query.CountAsync(ct);
        var products = await query.Skip((request.PageNumber - 1) * request.PageSize)
                                  .Take(request.PageSize)
                                  .ProjectToProduct()
                                  .ToListAsync(ct);

        return new PaginatedList<ProductRepresentation>(products, totalCount, request.PageNumber, request.PageSize);
    }
}
