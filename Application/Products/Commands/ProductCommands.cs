using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Products.Commands;

public record DeleteProductCommand(int ProductId, int? RequestingUserId = null, bool IsAdmin = false) : IRequest<bool>;

public class DeleteProductCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteProductCommand, bool>
{
    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        if (product == null) return false;

        if (!request.IsAdmin && !product.Categories.Any(c => c.PartnerId == request.RequestingUserId)) return false;

        context.Products.Remove(product);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record ProductSizeStockDto(ProductSize Size, int Stock);
public record SetProductSizesCommand(int ProductId, List<ProductSizeStockDto> Sizes, int? RequestingUserId = null, bool IsAdmin = false) : IRequest<bool>;

public class SetProductSizesCommandHandler(IApplicationDbContext context) : IRequestHandler<SetProductSizesCommand, bool>
{
    public async Task<bool> Handle(SetProductSizesCommand request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
        if (product == null) return false;

        if (!request.IsAdmin && !product.Categories.Any(c => c.PartnerId == request.RequestingUserId)) return false;

        var existing = context.ProductSizeStocks.Where(s => s.ProductId == request.ProductId);
        context.ProductSizeStocks.RemoveRange(existing);

        foreach (var s in request.Sizes)
            context.ProductSizeStocks.Add(new ProductSizeStock { ProductId = request.ProductId, Size = s.Size, Stock = s.Stock });

        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record CreatePartnerProductCommand(string Name, string? Description, decimal Price, Gender Gender, List<int> CategoryIds, int PartnerId) : IRequest<int>;

public class CreatePartnerProductCommandHandler(IApplicationDbContext context) : IRequestHandler<CreatePartnerProductCommand, int>
{
    public async Task<int> Handle(CreatePartnerProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Gender = request.Gender
        };

        if (request.CategoryIds.Count > 0)
        {
            var categories = await context.Categories
                .Where(c => request.CategoryIds.Contains(c.Id))
                .ToListAsync(cancellationToken);

            // Validation: all categories must belong to the partner
            if (categories.Any(c => c.PartnerId != request.PartnerId))
                throw new Exception("One or more categories do not belong to you.");

            foreach (var category in categories)
                product.Categories.Add(category);
        }

        context.Products.Add(product);
        await context.SaveChangesAsync(cancellationToken);
        return product.Id;
    }
}
