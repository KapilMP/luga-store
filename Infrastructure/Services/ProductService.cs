using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Products.Commands;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Infrastructure.Services;

public class ProductService(IApplicationDbContext context) : IProductService
{
    public async Task<int> CreateProductAsync(string name, string? description, decimal price, Gender gender, List<int> categoryIds, int? partnerId = null, CancellationToken ct = default)
    {
        var product = new Product
        {
            Name = name,
            Description = description,
            Price = price,
            Gender = gender
        };

        if (categoryIds.Count > 0)
        {
            var categories = await context.Categories
                .Where(c => categoryIds.Contains(c.Id))
                .ToListAsync(ct);

            // Validation if partner context is provided
            if (partnerId.HasValue && categories.Any(c => c.PartnerId != partnerId.Value))
                throw new Exception("One or more categories do not belong to you.");

            foreach (var category in categories)
                product.Categories.Add(category);
        }

        context.Products.Add(product);
        await context.SaveChangesAsync(ct);
        return product.Id;
    }

    public async Task<bool> SetProductSizesAsync(int productId, List<ProductSizeStockDto> sizes, int? requestingUserId = null, bool isAdmin = false, CancellationToken ct = default)
    {
        var product = await context.Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == productId, ct);

        if (product == null) return false;

        // Authorization check
        if (!isAdmin && !product.Categories.Any(c => c.PartnerId == requestingUserId)) 
            return false;

        var existing = context.ProductSizeStocks.Where(s => s.ProductId == productId);
        context.ProductSizeStocks.RemoveRange(existing);

        foreach (var s in sizes)
        {
            context.ProductSizeStocks.Add(new ProductSizeStock 
            { 
                ProductId = productId, 
                Size = s.Size, 
                Stock = s.Stock 
            });
        }

        await context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteProductAsync(int productId, int? requestingUserId = null, bool isAdmin = false, CancellationToken ct = default)
    {
        var product = await context.Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == productId, ct);

        if (product == null) return false;

        // Authorization check
        if (!isAdmin && !product.Categories.Any(c => c.PartnerId == requestingUserId)) 
            return false;

        context.Products.Remove(product);
        await context.SaveChangesAsync(ct);
        return true;
    }
}
