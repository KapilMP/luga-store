namespace SedaWears.Application.Features.Cart.Models;

public record CartItemDto(int Id, int ProductId, string Name, decimal Price, string Size, int Quantity, decimal Subtotal);
