using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Orders.Commands;

public record UpdateOrderStatusCommand(int OrderId, int CustomerId, OrderStatus Status) : IRequest;

public class UpdateOrderStatusCommandHandler(IOrderService orderService) : IRequestHandler<UpdateOrderStatusCommand>
{
    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        => await orderService.UpdateOrderStatusAsync(request.OrderId, request.CustomerId, request.Status, cancellationToken);
}
