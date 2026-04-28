using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Products.Models;

public record ProductRepresentation(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    decimal ShippingCost,
    Gender Gender,
    bool IsFeatured,
    bool IsNew,
    List<ProductSizeRepresentation> Sizes,
    List<ProductImageRepresentation> Images);

public record ProductSizeRepresentation(ProductSize Size, int Stock);
public record ProductImageRepresentation(int Id, string FileName, int Order);
public record ProductSaleRepresentation(int Id, decimal DiscountedPrice, decimal? DiscountPercent, DateTime StartsAt, DateTime? EndsAt, bool IsActive);
