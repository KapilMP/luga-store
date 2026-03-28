using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Orders.Commands;

public record CheckoutAddressDto(string FullName, string Phone, string Street, string City, string ZipCode);
public record CheckoutItemDto(int ProductId, int Quantity);
public record CheckoutCommand(
    int? UserId,
    string? CustomerEmail,
    CheckoutAddressDto? ShippingAddress,
    List<CheckoutItemDto> Items) : IRequest<CheckoutResult>;

public record CheckoutResult(int OrderId, string Status, decimal Total);

public class CheckoutCommandHandler(IOrderService orderService) : IRequestHandler<CheckoutCommand, CheckoutResult>
{
    public async Task<CheckoutResult> Handle(CheckoutCommand request, CancellationToken ct)
        => await orderService.CheckoutAsync(request, ct);
}
