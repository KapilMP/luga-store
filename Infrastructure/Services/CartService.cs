using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Infrastructure.Services;

public class CartService(IApplicationDbContext context) : ICartService
{
    public async Task AddToCartAsync(int userId, int productId, ProductSize size, int quantity, CancellationToken ct = default)
    {
        var existing = await context.CartItems
            .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId && c.Size == size, ct);

        if (existing != null)
        {
            existing.Quantity += quantity;
        }
        else
        {
            context.CartItems.Add(new CartItem 
            { 
                UserId = userId, 
                ProductId = productId, 
                Size = size, 
                Quantity = quantity 
            });
        }

        await context.SaveChangesAsync(ct);
    }

    public async Task<bool> UpdateCartItemAsync(int itemId, int userId, ProductSize size, int quantity, CancellationToken ct = default)
    {
        var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == itemId && c.UserId == userId, ct);
        if (item == null) return false;

        item.Size = size;
        item.Quantity = quantity;
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> RemoveCartItemAsync(int itemId, int userId, CancellationToken ct = default)
    {
        var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == itemId && c.UserId == userId, ct);
        if (item == null) return false;

        context.CartItems.Remove(item);
        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task ClearCartAsync(int userId, CancellationToken ct = default)
    {
        var items = context.CartItems.Where(c => c.UserId == userId);
        context.CartItems.RemoveRange(items);
        await context.SaveChangesAsync(ct);
    }
}
