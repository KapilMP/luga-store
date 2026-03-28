using LugaStore.Domain.Enums;

namespace LugaStore.Application.Cart;

// Request Models
public record AddToCartRequest(int ProductId, ProductSize Size, int Quantity);
public record UpdateCartRequest(ProductSize Size, int Quantity);

// Response Models
public record CartItemResponseDto(
    int Id,
    int ProductId,
    string Name,
    decimal Price,
    string Size,
    int Quantity,
    decimal Subtotal);
