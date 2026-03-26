namespace LugaStore.WebAPI.Dtos;

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
