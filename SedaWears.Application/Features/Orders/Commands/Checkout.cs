using FluentValidation;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Application.Features.Orders.Models;

namespace SedaWears.Application.Features.Orders.Commands;

public record CheckoutAddress(string FullName, string Phone, string Street, string City, string ZipCode);
public record CheckoutItem(int ProductId, int Quantity);

public record CheckoutCommand(
    string? CustomerEmail,
    CheckoutAddress? ShippingAddress,
    List<CheckoutItem> Items) : ICommand<CheckoutDto>;

public class CheckoutValidator : AbstractValidator<CheckoutCommand>
{
    public CheckoutValidator()
    {
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Your cart is empty.");

        RuleForEach(x => x.Items).ChildRules(item => {
            item.RuleFor(x => x.ProductId)
                .GreaterThan(0).WithMessage("Invalid product selected.");
            item.RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be at least 1.");
        });

        RuleFor(x => x.CustomerEmail)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");

        RuleFor(x => x.ShippingAddress)
            .NotNull().WithMessage("Shipping address is required.");

        RuleFor(x => x.ShippingAddress!.FullName)
            .NotEmpty().When(x => x.ShippingAddress != null).WithMessage("Full name is required.");
        RuleFor(x => x.ShippingAddress!.Phone)
            .NotEmpty().When(x => x.ShippingAddress != null).WithMessage("Phone number is required.");
        RuleFor(x => x.ShippingAddress!.Street)
            .NotEmpty().When(x => x.ShippingAddress != null).WithMessage("Street address is required.");
        RuleFor(x => x.ShippingAddress!.City)
            .NotEmpty().When(x => x.ShippingAddress != null).WithMessage("City is required.");
        RuleFor(x => x.ShippingAddress!.ZipCode)
            .NotEmpty().When(x => x.ShippingAddress != null).WithMessage("Zip code is required.");
    }
}

public class CheckoutHandler(IApplicationDbContext context, IAuthService authService, ICurrentUser currentUser) : ICommandHandler<CheckoutCommand, CheckoutDto>
{
    public async Task<CheckoutDto> Handle(CheckoutCommand request, CancellationToken ct)
    {
        int userId;

        if (currentUser.Id.HasValue)
        {
            userId = currentUser.Id!.Value;
        }
        else
        {
            if (string.IsNullOrEmpty(request.CustomerEmail)) throw new BadRequestException("Email is required for guest checkout.");

            var user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.CustomerEmail, ct);

            if (user == null)
            {
                var names = (request.ShippingAddress?.FullName ?? string.Empty).Split(' ', 2);
                var firstName = names.Length > 0 ? names[0] : string.Empty;
                var lastName = names.Length > 1 ? names[1] : string.Empty;

                var success = await authService.GuestCheckoutAsync(
                    request.CustomerEmail,
                    firstName,
                    lastName,
                    request.ShippingAddress?.Phone ?? string.Empty,
                    ct);

                if (!success) throw new InternalServerException("Guest checkout registration failed.");

                user = await context.Users.FirstOrDefaultAsync(u => u.Email == request.CustomerEmail, ct);
                if (user == null) throw new InternalServerException("Failed to resolve guest user.");
            }

            userId = user.Id;
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
                    throw new BadRequestException("Users can have at most 5 addresses saved.");

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
            if (product == null) throw new NotFoundException($"Product {item.ProductId} not found.");

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

        return new CheckoutDto(order.Id, order.Status.ToString(), order.TotalAmount);
    }
}
