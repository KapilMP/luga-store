using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Features.Products.Models;

namespace LugaStore.Application.Features.Products.Queries;

public record GetProductQuery(int Id) : IRequest<ProductDto>;

public class GetProductHandler(IApplicationDbContext dbContext) : IRequestHandler<GetProductQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductQuery request, CancellationToken ct)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(p => p.SizeStocks)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct) ?? throw new NotFoundError("Product not found");
            
        return new ProductDto(product.Id, product.Name, product.Description, product.Price, product.SizeStocks.Select(s => new ProductSizeDto(s.Size, s.Stock)).ToList());
    }
}
