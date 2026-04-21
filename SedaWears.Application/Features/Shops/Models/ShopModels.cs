using SedaWears.Application.Features.Users.Models;

namespace SedaWears.Application.Features.Shops.Models;

public record ShopRepresentation(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string? LogoUrl,
    bool IsActive
);
