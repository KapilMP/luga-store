using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Products.Queries;

public record GetProductsQuery : IRequest<List<ProductBrowseDto>>;

public class GetProductsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetProductsQuery, List<ProductBrowseDto>>
{
    public async Task<List<ProductBrowseDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        return await context.Products
            .AsNoTracking()
            .Include(p => p.Categories)
                .ThenInclude(c => c.Partner)
            .Include(p => p.SizeStocks)
            .Select(p => new ProductBrowseDto(
                p.Id,
                p.Name,
                p.Price,
                p.Description,
                p.Categories.Any(c => c.PartnerId != null),
                p.Categories.Any(c => c.Partner != null)
                    ? $"{p.Categories.First(c => c.Partner != null).Partner!.FirstName} {p.Categories.First(c => c.Partner != null).Partner!.LastName}"
                    : "Luga Brand",
                p.SizeStocks.Select(s => new ProductSizeStockResponseDto(s.Size, s.Stock))
            ))
            .ToListAsync(cancellationToken);
    }
}
