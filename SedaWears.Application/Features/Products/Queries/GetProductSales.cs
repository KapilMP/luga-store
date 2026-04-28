using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Products.Models;

namespace SedaWears.Application.Features.Products.Queries;

public record GetProductSalesQuery(int ProductId) : IRequest<List<ProductSaleRepresentation>>;

public class GetProductSalesHandler(IApplicationDbContext dbContext) : IRequestHandler<GetProductSalesQuery, List<ProductSaleRepresentation>>
{
    public async Task<List<ProductSaleRepresentation>> Handle(GetProductSalesQuery request, CancellationToken ct)
        => await dbContext.ProductSales
            .AsNoTracking()
            .Where(s => s.ProductId == request.ProductId)
            .Select(s => new ProductSaleRepresentation(s.Id, s.DiscountedPrice, s.DiscountPercent, s.StartsAt, s.EndsAt, s.IsActive))
            .ToListAsync(ct);
}
