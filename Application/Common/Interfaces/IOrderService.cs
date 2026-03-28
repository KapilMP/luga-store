using LugaStore.Domain.Entities;
using LugaStore.Application.Orders.Commands;

namespace LugaStore.Application.Common.Interfaces;

public interface IOrderService
{
    Task<List<Order>> GetOrdersByUserAsync(int userId, CancellationToken cancellationToken = default);
    Task UpdateOrderStatusAsync(int orderId, int customerId, OrderStatus status, CancellationToken cancellationToken = default);
    Task<CheckoutResult> CheckoutAsync(CheckoutCommand command, CancellationToken ct = default);
}
