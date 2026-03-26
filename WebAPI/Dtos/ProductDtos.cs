using LugaStore.Domain.Enums;

namespace LugaStore.WebAPI.Dtos;

public record ProductSizeStockResponseDto(ProductSize Size, int Stock);

public record ProductSaleDto(
    decimal DiscountedPrice,
    decimal? DiscountPercent,
    DateTime StartsAt,
    DateTime? EndsAt);

public record ProductImageDto(string Url, int DisplayOrder);

public record ProductSummaryDto(
    int Id,
    string Name,
    decimal Price,
    string? Description,
    IEnumerable<ProductSizeStockResponseDto> Sizes,
    IEnumerable<ProductImageDto> Images,
    ProductSaleDto? ActiveSale);

public record ProductDetailDto(
    int Id,
    string Name,
    string? Description,
    decimal Price,
    decimal ShippingCost,
    string Category,
    bool IsFeatured,
    bool IsNew,
    IEnumerable<ProductSizeStockResponseDto> Sizes,
    IEnumerable<ProductImageDto> Images,
    ProductSaleDto? ActiveSale);

public record CreatedProductDto(int Id);
