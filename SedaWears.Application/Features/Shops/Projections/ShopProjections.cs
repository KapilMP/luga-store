using SedaWears.Application.Features.Shops.Models;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Shops.Projections;

public static class ShopProjections
{
    public static IQueryable<ShopRepresentation> ProjectToShop(this IQueryable<Shop> query)
    {
        return query.Select(s => new ShopRepresentation(
            s.Id,
            s.Name,
            s.SubdomainSlug,
            s.Description,
            s.LogoFileName,
            s.BannerFileName,
            s.IsActive,
            s.CreatedAt
        ));
    }
}
