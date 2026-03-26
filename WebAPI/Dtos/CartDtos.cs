namespace LugaStore.WebAPI.Dtos;

public record CartItemResponseDto(
    int Id,
    int ProductId,
    string Name,
    decimal Price,
    string Size,
    int Quantity,
    decimal Subtotal);
