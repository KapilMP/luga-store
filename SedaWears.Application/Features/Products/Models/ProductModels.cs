using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Products.Models;

public record ProductRepresentation(int Id, string Name, string? Description, decimal Price, Gender Gender, List<string> Images, List<ProductSizeRepresentation> Sizes, DateTime CreatedAt);
public record ProductSizeRepresentation(ProductSize Size, int Stock);
