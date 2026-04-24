
namespace SedaWears.Application.Features.Shops.Models;

public record ShopRepresentation(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string? LogoFileName,
    string? BannerFileName,
    bool IsActive,
    bool? IsDeleted,
    DateTime CreatedAt
);