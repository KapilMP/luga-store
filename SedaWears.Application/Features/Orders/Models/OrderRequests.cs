namespace SedaWears.Application.Features.Orders.Models;

public record CheckoutAddressRequest(string FullName, string Phone, string Street, string City, string ZipCode);
public record OrderItemRequest(int ProductId, int Quantity);
public record CreateOrderRequest(string? CustomerEmail, CheckoutAddressRequest? ShippingAddress, List<OrderItemRequest> Items);
