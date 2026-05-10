using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Features.Products.Models;

namespace SedaWears.Application.Features.Products.Queries;

public record GetProductQuery(int Id) : IRequest<ProductRepresentation>;

public class GetProductHandler(IApplicationDbContext dbContext) : IRequestHandler<GetProductQuery, ProductRepresentation>
{
    public async Task<ProductRepresentation> Handle(GetProductQuery request, CancellationToken ct)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Include(p => p.Images)
            .Include(p => p.SizeStocks)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct) ?? throw new NotFoundException("Product not found");
            
        return new ProductRepresentation(
            product.Id, 
            product.Name, 
            product.Description, 
            product.Price, 
            product.Gender, 
            product.Images.OrderBy(i => i.Order).Select(i => i.FileName).ToList(),
            product.SizeStocks.Select(s => new ProductSizeRepresentation(s.Size, s.Stock)).ToList(),
            product.CreatedAt);
    }
}
