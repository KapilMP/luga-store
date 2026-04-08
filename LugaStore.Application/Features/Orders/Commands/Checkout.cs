using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Features.Orders.Commands;

public record CheckoutAddressDto(string FullName, string Phone, string Street, string City, string ZipCode);
public record CheckoutItemDto(int ProductId, int Quantity);
public record CheckoutResult(int OrderId, string Status, decimal Total);

public record CheckoutCommand(
    int? UserId,
    string? CustomerEmail,
    CheckoutAddressDto? ShippingAddress,
    List<CheckoutItemDto> Items) : ICommand<CheckoutResult>;

public class CheckoutHandler(IApplicationDbContext context, IAuthService authService) : ICommandHandler<CheckoutCommand, CheckoutResult>
{
    public async Task<CheckoutResult> Handle(CheckoutCommand request, CancellationToken ct)
    {
        int userId;

        if (request.UserId.HasValue)
        {
            userId = request.UserId.Value;
        }
        else
        {
            if (string.IsNullOrEmpty(request.CustomerEmail)) throw new BadRequestError("Email is required for guest checkout.");

            var success = await authService.GuestCheckoutAsync(
                request.CustomerEmail,
                request.ShippingAddress?.FullName ?? string.Empty,
                string.Empty,
                request.ShippingAddress?.Phone ?? string.Empty,
                ct);

            if (!success) throw new InternalServerError("Guest checkout registration failed.");

            var guestUser = await context.Users.FirstOrDefaultAsync(u => u.Email == request.CustomerEmail, ct);
            if (guestUser == null) throw new InternalServerError("Failed to resolve guest user.");
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
                var addressCount = await context.Addresses.CountAsync(a => a.UserId == userId, ct);
                if (addressCount >= 5)
                    throw new BadRequestError("Users can have at most 5 addresses saved.");

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
            if (product == null) throw new NotFoundError($"Product {item.ProductId} not found.");

            order.Items.Add(new OrderItem 
            { 
                Product = product, 
                Quantity = item.Quantity, 
                UnitPrice = product.Price 
            });
            order.TotalAmount += product.Price * item.Quantity;
        }

        context.Orders.Add(order);
        await context.SaveChangesAsync(ct);

        return new CheckoutResult(order.Id, order.Status.ToString(), order.TotalAmount);
    }
}
