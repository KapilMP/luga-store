using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Products.Models;

public record ProductRepresentation(int Id, string Name, string? Description, decimal Price, List<ProductSizeRepresentation> Sizes);
public record ProductSizeRepresentation(ProductSize Size, int Stock);
