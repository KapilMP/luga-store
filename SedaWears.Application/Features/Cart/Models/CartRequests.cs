using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Cart.Models;

public record AddToCartRequest(int ProductId, ProductSize Size, int Quantity);
public record UpdateCartRequest(ProductSize Size, int Quantity);
