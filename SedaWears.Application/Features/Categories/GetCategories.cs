using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Categories;

public record CategoryDto(int Id, string Name, string Slug, string? Description, int DisplayOrder);

public record GetCategoriesQuery(int? ShopId = null) : IRequest<List<CategoryDto>>;

public class GetCategoriesHandler(IApplicationDbContext dbContext) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var query = dbContext.Categories.AsNoTracking();

        if (request.ShopId.HasValue)
            query = query.Where(c => c.ShopId == request.ShopId);
        else 
            query = query.Where(c => c.ShopId == null);

        return await query
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.Description, c.DisplayOrder))
            .ToListAsync(ct);
    }
}
