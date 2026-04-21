using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Products.Commands;

public record ProductSizeCommandDto(ProductSize Size, int Stock);

public record CreateProductCommand(string Name, string? Description, decimal Price, int CategoryId, List<ProductSizeCommandDto> Sizes) : IRequest<int>;

public class CreateProductHandler(IApplicationDbContext dbContext) : IRequestHandler<CreateProductCommand, int>
{
    public async Task<int> Handle(CreateProductCommand request, CancellationToken ct)
    {
        var product = new Product 
        { 
            Name = request.Name, 
            Description = request.Description, 
            Price = request.Price,
            CategoryId = request.CategoryId,
            SizeStocks = request.Sizes.Select(s => new ProductSizeStock { Size = s.Size, Stock = s.Stock }).ToList()
        };
        
        dbContext.Products.Add(product);
        await dbContext.SaveChangesAsync(ct);
        return product.Id;
    }
}
