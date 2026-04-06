using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Cart.Queries;

public class GetCartQueryHandler(IApplicationDbContext context) : IQueryHandler<GetCartQuery, List<CartItemDto>>
{
    public async Task<List<CartItemDto>> Handle(GetCartQuery request, CancellationToken cancellationToken)
        => await context.CartItems
            .Where(c => c.UserId == request.UserId)
            .Include(c => c.Product)
            .Select(c => new CartItemDto(
                c.Id,
                c.ProductId,
                c.Product.Name,
                c.Product.Price,
                c.Size.ToString(),
                c.Quantity,
                c.Product.Price * c.Quantity))
            .ToListAsync(cancellationToken);
}
