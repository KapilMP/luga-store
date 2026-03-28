namespace LugaStore.Application.Orders;

// Request Models
public record CheckoutAddressRequest(string FullName, string Phone, string Street, string City, string ZipCode);
public record CreateOrderRequest(string? CustomerEmail, CheckoutAddressRequest? ShippingAddress, List<OrderItemRequestDto> Items);
public record OrderItemRequestDto(int ProductId, int Quantity);

// Response Models
public record OrderItemResponseDto(
    int ProductId,
    string ProductName,
    int Quantity,
    decimal UnitPrice,
    decimal Subtotal);

public record OrderResponseDto(
    int Id,
    string Status,
    decimal Total,
    DateTime CreatedAt,
    IEnumerable<OrderItemResponseDto> Items);

public record CheckoutResponseDto(int OrderId, string Status, decimal Total);
