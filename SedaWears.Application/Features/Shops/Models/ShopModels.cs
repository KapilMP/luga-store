
namespace SedaWears.Application.Features.Shops.Models;

public record ShopRepresentation(
    int Id,
    string Name,
    string SubdomainSlug,
    string? Description,
    string? LogoFileName,
    string? BannerFileName,
    bool IsActive,
    DateTime CreatedAt
);