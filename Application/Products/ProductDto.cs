using LugaStore.Domain.Enums;
using LugaStore.Application.Products.Commands;

namespace LugaStore.Application.Products;

// Request Models (Inputs)
public record ProductUpsertRequest(string Name, string? Description, decimal Price, Gender Gender, List<int> CategoryIds);
public record SetSizesRequest(List<ProductSizeStockDto> Sizes);

// Response Models (Outputs)
public record ProductDto(int Id, string Name, string? Description, decimal Price, int DisplayOrder);
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
