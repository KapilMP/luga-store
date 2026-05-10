using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Categories.Models;
using SedaWears.Application.Features.Categories.Projections;

using SedaWears.Application.Common.Validators;

namespace SedaWears.Application.Features.Categories.Queries;

public record GetCategoriesQuery(
    int? ShopId = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = null,
    string? SortOrder = "asc",
    string? Search = null) : IRequest<PaginatedList<CategoryRepresentation>>, IPaginatedQuery;

public class GetCategoriesValidator : PaginatedQueryValidator<GetCategoriesQuery> { }

public class GetCategoriesHandler(IApplicationDbContext dbContext) : IRequestHandler<GetCategoriesQuery, PaginatedList<CategoryRepresentation>>
{
    public async Task<PaginatedList<CategoryRepresentation>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var query = dbContext.Categories
            .AsNoTracking();

        if (request.ShopId.HasValue)
            query = query.Where(c => c.ShopId == request.ShopId);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var searchTerm = request.Search.Trim();
            query = query.Where(c => EF.Functions.ILike(c.Name, $"%{searchTerm}%"));
        }

        var isDescending = request.SortOrder?.ToLower() == "desc";
        query = (request.SortBy?.ToLower()) switch
        {
            "name" => isDescending ? query.OrderByDescending(c => c.Name) : query.OrderBy(c => c.Name),
            "isactive" => isDescending ? query.OrderByDescending(c => c.IsActive) : query.OrderBy(c => c.IsActive),
            "displayorder" => isDescending ? query.OrderByDescending(c => c.DisplayOrder) : query.OrderBy(c => c.DisplayOrder),
            _ => isDescending ? query.OrderByDescending(c => c.DisplayOrder) : query.OrderBy(c => c.DisplayOrder)
        };

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ProjectToCategory()
            .ToListAsync(ct);

        return new PaginatedList<CategoryRepresentation>(items, totalCount, request.PageNumber, request.PageSize);
    }
}
