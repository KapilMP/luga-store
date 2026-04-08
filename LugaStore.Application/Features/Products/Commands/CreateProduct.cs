using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Features.Products.Commands;

public record ProductSizeCommandDto(ProductSize Size, int Stock);

public record CreateProductCommand(string Name, string? Description, decimal Price, List<ProductSizeCommandDto> Sizes) : IRequest<int>;

public class CreateProductHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product 
        { 
            Name = request.Name, 
            Description = request.Description, 
            Price = request.Price,
            SizeStocks = request.Sizes.Select(s => new ProductSizeStock { Size = s.Size, Stock = s.Stock }).ToList()
        };
        
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(ct);
        return product.Id;
    }
}
