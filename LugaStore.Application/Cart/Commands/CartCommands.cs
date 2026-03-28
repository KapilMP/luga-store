using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Cart.Commands;

public record AddToCartCommand(int UserId, int ProductId, ProductSize Size, int Quantity) : IRequest;

public class AddToCartCommandHandler(IApplicationDbContext context) : IRequestHandler<AddToCartCommand>
{
    public async Task Handle(AddToCartCommand request, CancellationToken ct)
    {
        var productExists = await context.Products.AnyAsync(p => p.Id == request.ProductId, ct);
        if (!productExists) throw new NotFoundError("Product not found");

        var existing = await context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == request.UserId && c.ProductId == request.ProductId && c.Size == request.Size, ct);

        if (existing != null)
        {
            existing.Quantity += request.Quantity;
        }
        else
        {
            context.CartItems.Add(new CartItem 
            { 
                UserId = request.UserId, 
                ProductId = request.ProductId, 
                Size = request.Size, 
                Quantity = request.Quantity 
            });
        }

        await context.SaveChangesAsync(ct);
    }
}

public record UpdateCartItemCommand(int ItemId, int UserId, ProductSize Size, int Quantity) : IRequest;

public class UpdateCartItemCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateCartItemCommand>
{
    public async Task Handle(UpdateCartItemCommand request, CancellationToken ct)
    {
        var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == request.ItemId && c.UserId == request.UserId, ct);
        if (item == null) throw new NotFoundError("Cart item not found");

        item.Size = request.Size;
        item.Quantity = request.Quantity;
        await context.SaveChangesAsync(ct);
    }
}

public record RemoveCartItemCommand(int ItemId, int UserId) : IRequest;

public class RemoveCartItemCommandHandler(IApplicationDbContext context) : IRequestHandler<RemoveCartItemCommand>
{
    public async Task Handle(RemoveCartItemCommand request, CancellationToken ct)
    {
        var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == request.ItemId && c.UserId == request.UserId, ct);
        if (item == null) throw new NotFoundError("Cart item not found");

        context.CartItems.Remove(item);
        await context.SaveChangesAsync(ct);
    }
}

public record ClearCartCommand(int UserId) : IRequest;

public class ClearCartCommandHandler(IApplicationDbContext context) : IRequestHandler<ClearCartCommand>
{
    public async Task Handle(ClearCartCommand request, CancellationToken ct)
    {
        var items = context.CartItems.Where(c => c.UserId == request.UserId);
        context.CartItems.RemoveRange(items);
        await context.SaveChangesAsync(ct);
    }
}
