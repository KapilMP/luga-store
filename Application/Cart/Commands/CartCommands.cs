using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace LugaStore.Application.Cart.Commands;

public record AddToCartCommand(int UserId, int ProductId, ProductSize Size, int Quantity) : IRequest;

public class AddToCartCommandHandler(IApplicationDbContext context) : IRequestHandler<AddToCartCommand>
{
    public async Task Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var existing = await context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == request.UserId && c.ProductId == request.ProductId && c.Size == request.Size, cancellationToken);

        if (existing != null)
            existing.Quantity += request.Quantity;
        else
            context.CartItems.Add(new CartItem { UserId = request.UserId, ProductId = request.ProductId, Size = request.Size, Quantity = request.Quantity });

        await context.SaveChangesAsync(cancellationToken);
    }
}

public record UpdateCartItemCommand(int ItemId, int UserId, ProductSize Size, int Quantity) : IRequest<bool>;

public class UpdateCartItemCommandHandler(IApplicationDbContext context) : IRequestHandler<UpdateCartItemCommand, bool>
{
    public async Task<bool> Handle(UpdateCartItemCommand request, CancellationToken cancellationToken)
    {
        var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == request.ItemId && c.UserId == request.UserId, cancellationToken);
        if (item == null) return false;

        item.Size = request.Size;
        item.Quantity = request.Quantity;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record RemoveCartItemCommand(int ItemId, int UserId) : IRequest<bool>;

public class RemoveCartItemCommandHandler(IApplicationDbContext context) : IRequestHandler<RemoveCartItemCommand, bool>
{
    public async Task<bool> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == request.ItemId && c.UserId == request.UserId, cancellationToken);
        if (item == null) return false;

        context.CartItems.Remove(item);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record ClearCartCommand(int UserId) : IRequest;

public class ClearCartCommandHandler(IApplicationDbContext context) : IRequestHandler<ClearCartCommand>
{
    public async Task Handle(ClearCartCommand request, CancellationToken cancellationToken)
    {
        var items = context.CartItems.Where(c => c.UserId == request.UserId);
        context.CartItems.RemoveRange(items);
        await context.SaveChangesAsync(cancellationToken);
    }
}
