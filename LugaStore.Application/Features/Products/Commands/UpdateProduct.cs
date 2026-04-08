using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Features.Products.Commands;

public record UpdateProductCommand(int Id, string Name, string? Description, decimal Price, List<ProductSizeCommandDto> Sizes) : IRequest<Unit>;

public class UpdateProductHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateProductCommand, Unit>
{
    public async Task<Unit> Handle(UpdateProductCommand request, CancellationToken ct)
    {
        var product = await dbContext.Products
            .Include(p => p.SizeStocks)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct) ?? throw new NotFoundError("Product not found");

        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;

        // Sync sizes
        product.SizeStocks.Clear();
        foreach (var s in request.Sizes)
        {
            product.SizeStocks.Add(new ProductSizeStock { Size = s.Size, Stock = s.Stock });
        }

        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
