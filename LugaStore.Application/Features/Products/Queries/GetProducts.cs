using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Features.Products.Models;

namespace LugaStore.Application.Features.Products.Queries;

public record GetProductsQuery(int PageNumber = 1, int PageSize = 10, int? CategoryId = null, string? Search = null) : IRequest<PaginatedList<ProductDto>>;

public class GetProductsHandler(IApplicationDbContext dbContext) : IRequestHandler<GetProductsQuery, PaginatedList<ProductDto>>
{
    public async Task<PaginatedList<ProductDto>> Handle(GetProductsQuery request, CancellationToken ct)
    {
        var query = dbContext.Products.AsNoTracking();
        if (request.CategoryId.HasValue) query = query.Where(p => p.Categories.Any(c => c.Id == request.CategoryId));
        if (!string.IsNullOrEmpty(request.Search)) query = query.Where(p => p.Name.Contains(request.Search));

        return await query
            .OrderBy(p => p.Name)
            .Select(p => new ProductDto(p.Id, p.Name, p.Description, p.Price, p.SizeStocks.Select(s => new ProductSizeDto(s.Size, s.Stock)).ToList()))
            .PaginatedListAsync(request.PageNumber, request.PageSize, ct);
    }
}
