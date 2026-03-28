using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Orders.Commands;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Services;

public class OrderService(IApplicationDbContext context, IAuthService authService) : IOrderService
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

    public async Task<CheckoutResult> CheckoutAsync(CheckoutCommand request, CancellationToken ct = default)
    {
        int userId;

        if (request.UserId.HasValue)
        {
            userId = request.UserId.Value;
        }
        else
        {
            await authService.GuestCheckoutAsync(
                request.CustomerEmail!,
                request.ShippingAddress?.FullName ?? string.Empty,
                string.Empty,
                request.ShippingAddress?.Phone ?? string.Empty);

            var guestUser = await context.Users.FirstOrDefaultAsync(u => u.Email == request.CustomerEmail, ct);
            if (guestUser == null) throw new InvalidOperationException("Failed to resolve guest user.");
            userId = guestUser.Id;
        }

        if (request.ShippingAddress != null)
        {
            var existingAddress = await context.Addresses
                .FirstOrDefaultAsync(a => a.UserId == userId && 
                    a.Street == request.ShippingAddress.Street && 
                    a.City == request.ShippingAddress.City && 
                    a.ZipCode == request.ShippingAddress.ZipCode, ct);

            if (existingAddress == null)
            {
                var count = await context.Addresses.CountAsync(a => a.UserId == userId, ct);
                if (count >= 5)
                    throw new BadRequestError("Users can have at most 5 addresses.");

                context.Addresses.Add(new Address
                {
                    UserId = userId,
                    FullName = request.ShippingAddress.FullName,
                    Email = request.CustomerEmail ?? string.Empty,
                    Phone = request.ShippingAddress.Phone,
                    Street = request.ShippingAddress.Street,
                    City = request.ShippingAddress.City,
                    ZipCode = request.ShippingAddress.ZipCode,
                    Label = "Shipping"
                });
            }
        }

        var order = new Order { UserId = userId, Status = OrderStatus.Pending, TotalAmount = 0 };

        foreach (var item in request.Items)
        {
            var product = await context.Products.FindAsync([item.ProductId], ct);
            if (product == null) throw new KeyNotFoundException($"Product {item.ProductId} not found.");

            order.Items.Add(new OrderItem { ProductId = product.Id, Quantity = item.Quantity, UnitPrice = product.Price });
            order.TotalAmount += product.Price * item.Quantity;
        }

        context.Orders.Add(order);
        await context.SaveChangesAsync(ct);

        return new CheckoutResult(order.Id, order.Status.ToString(), order.TotalAmount);
    }
}
