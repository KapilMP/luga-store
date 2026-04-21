using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Products.Models;

namespace SedaWears.Application.Features.Products.Queries;

public record GetProductsQuery(int PageNumber = 1, int PageSize = 10, int? CategoryId = null, string? Search = null) : IRequest<PaginatedList<ProductRepresentation>>;

public class GetProductsHandler(IApplicationDbContext dbContext) : IRequestHandler<GetProductsQuery, PaginatedList<ProductRepresentation>>
{
    public async Task<PaginatedList<ProductRepresentation>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var query = dbContext.Products.AsNoTracking();
        if (request.CategoryId.HasValue) query = query.Where(p => p.CategoryId == request.CategoryId);
        if (!string.IsNullOrEmpty(request.Search)) query = query.Where(p => p.Name.Contains(request.Search));

        query = query.OrderBy(p => p.Name);

        var totalCount = await query.CountAsync(ct);
        var products = await query.Skip((request.PageNumber - 1) * request.PageSize)
                                  .Take(request.PageSize)
                                  .Select(p => new ProductRepresentation(p.Id, p.Name, p.Description, p.Price, p.SizeStocks.Select(s => new ProductSizeRepresentation(s.Size, s.Stock)).ToList()))
                                  .ToListAsync(ct);

        return new PaginatedList<ProductRepresentation>(products, totalCount, request.PageNumber, request.PageSize);
    }
}
