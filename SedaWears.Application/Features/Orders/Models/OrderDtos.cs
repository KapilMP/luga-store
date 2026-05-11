namespace SedaWears.Application.Features.Orders.Models;

public record CheckoutDto(int OrderId, string Status, decimal Total);
