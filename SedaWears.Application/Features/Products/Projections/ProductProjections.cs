using SedaWears.Application.Features.Products.Models;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Products.Projections;

public static class ProductProjections
{
    public static IQueryable<ProductRepresentation> ProjectToProduct(this IQueryable<Product> query)
    {
        return query.Select(p => new ProductRepresentation(
            p.Id,
            p.Name,
            p.Description,
            p.Price,
            p.Gender,
            p.Images.Select(i => i.FileName).ToList(),
            p.SizeStocks.Select(s => new ProductSizeRepresentation(s.Size, s.Stock)).ToList(),
            p.CreatedAt
        ));
    }
}
