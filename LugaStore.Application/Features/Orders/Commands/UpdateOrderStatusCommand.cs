using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Orders.Commands;

public record UpdateOrderStatusCommand(int OrderId, int CustomerId, OrderStatus Status) : IRequest;

public class UpdateOrderStatusCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateOrderStatusCommand>
{
    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var existingOrder = await context.Orders
            .AsNoTracking()
            .Select(o => new { o.Id, o.UserId })
            .FirstOrDefaultAsync(o => o.Id == request.OrderId, cancellationToken)
            ?? throw new NotFoundError("Order not found.");

        if (existingOrder.UserId != request.CustomerId)
            throw new ForbiddenError("Order does not belong to this customer.");

        await context.Orders
            .Where(o => o.Id == request.OrderId)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.Status, request.Status), cancellationToken);
    }
}
