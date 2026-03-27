using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Services;

public class OrderService(IApplicationDbContext context) : IOrderService
{
    public async Task<List<Order>> GetOrdersByUserAsync(int userId, CancellationToken cancellationToken = default)
        => await context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.Created)
            .ToListAsync(cancellationToken);

    public async Task UpdateOrderStatusAsync(int orderId, int customerId, OrderStatus status, CancellationToken cancellationToken = default)
    {
        var order = await context.Orders.FindAsync([orderId], cancellationToken)
            ?? throw new NotFoundException("Order not found.");

        if (order.UserId != customerId)
            throw new ForbiddenException("Order does not belong to this customer.");

        order.Status = status;
        await context.SaveChangesAsync(cancellationToken);
    }
}
