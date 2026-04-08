using LugaStore.Domain.Enums;

namespace LugaStore.Application.Features.Products.Models;

public record ProductDto(int Id, string Name, string? Description, decimal Price, List<ProductSizeDto> Sizes);
public record ProductSizeDto(ProductSize Size, int Stock);
