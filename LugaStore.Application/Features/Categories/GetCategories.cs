using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Categories;

public record CategoryDto(int Id, string Name, string Slug, string? Description, int DisplayOrder);

public record GetCategoriesQuery(int? PartnerId = null) : IRequest<List<CategoryDto>>;

public class GetCategoriesHandler(IApplicationDbContext dbContext) : IRequestHandler<GetCategoriesQuery, List<CategoryDto>>
{
    public async Task<List<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken ct)
    {
        var query = dbContext.Categories.AsNoTracking();

        if (request.PartnerId.HasValue)
            query = query.Where(c => c.PartnerId == request.PartnerId);
        else 
            query = query.Where(c => c.PartnerId == null);

        return await query
            .OrderBy(c => c.DisplayOrder)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Slug, c.Description, c.DisplayOrder))
            .ToListAsync(ct);
    }
}
