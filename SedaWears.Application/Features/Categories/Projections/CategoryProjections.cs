using SedaWears.Application.Features.Categories.Models;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Categories.Projections;

public static class CategoryProjections
{
    public static IQueryable<CategoryRepresentation> ProjectToCategory(this IQueryable<Category> query)
    {
        return query.Select(c => new CategoryRepresentation(
            c.Id,
            c.Name,
            c.Description,
            c.DisplayOrder,
            c.IsActive
        ));
    }
}
