using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Services;

public class OrderService(IApplicationDbContext context) : IOrderService
{
    public async Task<List<Order>> GetOrdersByUserAsync(int userId, CancellationToken cancellationToken = default)
        => await context.Orders
            .AsNoTracking()
            .Where(o => o.UserId == userId)
            .Include(o => o.Items)
            .ThenInclude(i => i.Product)
            .OrderByDescending(o => o.Created)
            .ToListAsync(cancellationToken);

    public async Task UpdateOrderStatusAsync(int orderId, int customerId, OrderStatus status, CancellationToken cancellationToken = default)
    {
        var existingOrder = await context.Orders
            .AsNoTracking()
            .Select(o => new { o.Id, o.UserId })
            .FirstOrDefaultAsync(o => o.Id == orderId, cancellationToken)
            ?? throw new NotFoundError("Order not found.");

        if (existingOrder.UserId != customerId)
            throw new ForbiddenError("Order does not belong to this customer.");

        await context.Orders
            .Where(o => o.Id == orderId)
            .ExecuteUpdateAsync(s => s.SetProperty(o => o.Status, status), cancellationToken);
    }
}
