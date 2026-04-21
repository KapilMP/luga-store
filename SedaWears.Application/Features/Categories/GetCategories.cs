using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;

using SedaWears.Application.Features.Categories.Models;

namespace SedaWears.Application.Features.Categories;

public record GetCategoriesQuery(int? ShopId = null) : IRequest<List<CategoryRepresentation>>;

public class GetCategoriesHandler(IApplicationDbContext dbContext) : IRequestHandler<GetCategoriesQuery, List<CategoryRepresentation>>
{
    public async Task<List<CategoryRepresentation>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var query = dbContext.Categories.AsNoTracking();

        if (request.ShopId.HasValue)
            query = query.Where(c => c.ShopId == request.ShopId);
        else 
            query = query.Where(c => c.ShopId == null);

        return await query
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryRepresentation(c.Id, c.Name, c.Slug, c.Description, c.DisplayOrder))
            .ToListAsync(ct);
    }
}
