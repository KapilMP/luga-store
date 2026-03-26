using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Orders.Commands;

public record CheckoutAddressDto(string FullName, string Phone, string Street, string City, string ZipCode);
public record CheckoutItemDto(int ProductId, int Quantity);
public record CheckoutCommand(
    int? UserId,
    string? CustomerEmail,
    CheckoutAddressDto? ShippingAddress,
    List<CheckoutItemDto> Items) : IRequest<CheckoutResult>;

public record CheckoutResult(int OrderId, string Status, decimal Total);

public class CheckoutCommandHandler(IApplicationDbContext context, IAuthService authService) : IRequestHandler<CheckoutCommand, CheckoutResult>
{
    public async Task<CheckoutResult> Handle(CheckoutCommand request, CancellationToken cancellationToken)
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

            var guestUser = await context.Users.FirstOrDefaultAsync(u => u.Email == request.CustomerEmail, cancellationToken);
            if (guestUser == null) throw new InvalidOperationException("Failed to resolve guest user.");
            userId = guestUser.Id;
        }

        if (request.ShippingAddress != null)
        {
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

        var order = new Order { UserId = userId, Status = OrderStatus.Pending, TotalAmount = 0 };

        foreach (var item in request.Items)
        {
            var product = await context.Products.FindAsync([item.ProductId], cancellationToken);
            if (product == null) throw new KeyNotFoundException($"Product {item.ProductId} not found.");

            order.Items.Add(new OrderItem { ProductId = product.Id, Quantity = item.Quantity, UnitPrice = product.Price });
            order.TotalAmount += product.Price * item.Quantity;
        }

        context.Orders.Add(order);
        await context.SaveChangesAsync(cancellationToken);

        return new CheckoutResult(order.Id, order.Status.ToString(), order.TotalAmount);
    }
}
