using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Products.Queries;

public record GetAllProductsQuery : IRequest<List<Product>>;

public class GetAllProductsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetAllProductsQuery, List<Product>>
{
    public async Task<List<Product>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        => await context.Products
            .Include(p => p.Creator)
            .Include(p => p.SizeStocks)
            .ToListAsync(cancellationToken);
}

public record GetMyProductsQuery(int CreatorId) : IRequest<List<Product>>;

public class GetMyProductsQueryHandler(IApplicationDbContext context) : IRequestHandler<GetMyProductsQuery, List<Product>>
{
    public async Task<List<Product>> Handle(GetMyProductsQuery request, CancellationToken cancellationToken)
        => await context.Products
            .Where(p => p.CreatorId == request.CreatorId)
            .Include(p => p.SizeStocks)
            .ToListAsync(cancellationToken);
}

public record GetProductByIdQuery(int ProductId) : IRequest<Product?>;

public class GetProductByIdQueryHandler(IApplicationDbContext context) : IRequestHandler<GetProductByIdQuery, Product?>
{
    public async Task<Product?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        => await context.Products
            .Include(p => p.Creator)
            .Include(p => p.SizeStocks)
            .FirstOrDefaultAsync(p => p.Id == request.ProductId, cancellationToken);
}
