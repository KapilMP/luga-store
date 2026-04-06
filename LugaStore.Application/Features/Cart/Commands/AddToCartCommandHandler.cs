using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Cart.Commands;

public class AddToCartCommandHandler(IApplicationDbContext context) : ICommandHandler<AddToCartCommand>
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
