using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Features.Products.Commands;

public record DeleteProductCommand(int ProductId, int? RequestingUserId = null, bool IsAdmin = false) : IRequest;

public class DeleteProductCommandHandler(IApplicationDbContext context) : IRequestHandler<DeleteProductCommand>
{
    public async Task Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await context.Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product == null) throw new NotFoundError("Product not found");

        if (!request.IsAdmin && !product.Categories.Any(c => c.PartnerId == request.RequestingUserId)) 
            throw new ForbiddenError("You do not have permission to delete this product");

        context.Products.Remove(product);
        await context.SaveChangesAsync(ct);
    }
}

public record ProductSizeStockDto(ProductSize Size, int Stock);
public record SetProductSizesCommand(int ProductId, List<ProductSizeStockDto> Sizes, int? RequestingUserId = null, bool IsAdmin = false) : IRequest;

public class SetProductSizesCommandHandler(IApplicationDbContext context) : IRequestHandler<SetProductSizesCommand>
{
    public async Task Handle(SetProductSizesCommand request, CancellationToken ct)
    {
        var product = await context.Products
            .Include(p => p.Categories)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, ct);

        if (product == null) throw new NotFoundError("Product not found");

        if (!request.IsAdmin && !product.Categories.Any(c => c.PartnerId == request.RequestingUserId)) 
            throw new ForbiddenError("You do not have permission to update this product");

        var existing = context.ProductSizeStocks.Where(s => s.ProductId == request.ProductId);
        context.ProductSizeStocks.RemoveRange(existing);

        foreach (var s in request.Sizes)
        {
            context.ProductSizeStocks.Add(new ProductSizeStock 
            { 
                ProductId = request.ProductId, 
                Size = s.Size, 
                Stock = s.Stock 
            });
        }

        await context.SaveChangesAsync(ct);
    }
}

public record CreatePartnerProductCommand(string Name, string? Description, decimal Price, Gender Gender, List<int> CategoryIds, int PartnerId) : IRequest<int>;

public class CreatePartnerProductCommandHandler(IApplicationDbContext context) : IRequestHandler<CreatePartnerProductCommand, int>
{
    public async Task<int> Handle(CreatePartnerProductCommand request, CancellationToken ct)
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
                .ToListAsync(ct);

            if (categories.Any(c => c.PartnerId != request.PartnerId))
                throw new ForbiddenError("One or more categories do not belong to you.");

            foreach (var category in categories)
                product.Categories.Add(category);
        }

        context.Products.Add(product);
        await context.SaveChangesAsync(ct);
        return product.Id;
    }
}
