using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Products.Queries;

public record GetAllProductsQuery : IRequest<List<ProductBrowseDto>>;

public class GetAllProductsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetAllProductsQuery, List<ProductBrowseDto>>
{
    public async Task<List<ProductBrowseDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
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

public record GetMyProductsQuery(int PartnerId) : IRequest<List<ProductBrowseDto>>;

public class GetMyProductsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetMyProductsQuery, List<ProductBrowseDto>>
{
    public async Task<List<ProductBrowseDto>> Handle(GetMyProductsQuery request, CancellationToken cancellationToken)
    {
        return await context.Products
            .AsNoTracking()
            .Where(p => p.Categories.Any(c => c.PartnerId == request.PartnerId))
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

public record GetProductByIdQuery(int ProductId) : IRequest<ProductDetailDto>;

public class GetProductByIdQueryHandler(IApplicationDbContext context) : IRequestHandler<GetProductByIdQuery, ProductDetailDto>
{
    public async Task<ProductDetailDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await context.Products
            .AsNoTracking()
            .Include(p => p.Categories)
            .Include(p => p.SizeStocks)
            .Include(p => p.Images)
            .Include(p => p.Sales)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken)
            ?? throw new NotFoundError("Product not found");

        var activeSale = product.Sales.FirstOrDefault(s => s.StartsAt <= DateTime.UtcNow && (s.EndsAt == null || s.EndsAt >= DateTime.UtcNow));

        return new ProductDetailDto(
            product.Id,
            product.Name,
            product.Description,
            product.Price,
            product.ShippingCost,
            product.Categories.FirstOrDefault()?.Name ?? "General",
            product.IsFeatured,
            product.IsNew,
            product.SizeStocks.Select(s => new ProductSizeStockResponseDto(s.Size, s.Stock)),
            product.Images.Select(i => new ProductImageDto(i.ImagePath, i.DisplayOrder)),
            activeSale != null ? new ProductSaleDto(activeSale.DiscountedPrice, activeSale.DiscountPercent, activeSale.StartsAt, activeSale.EndsAt) : null
        );
    }
}
