namespace SedaWears.Application.Features.Orders.Models;

public record CheckoutRepresentation(int OrderId, string Status, decimal Total);
