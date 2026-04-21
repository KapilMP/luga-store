
namespace SedaWears.Application.Features.Shops.Models;

public record ShopRepresentation(
    int Id,
    string Name,
    string Slug,
    string? Description,
    string? LogoUrl,
    bool IsActive,
    DateTime CreatedAt,
    List<ShopOwnerSummary> Owners
);

public record ShopOwnerSummary(
    int Id,
    string FirstName,
    string LastName,
    string Email
);
