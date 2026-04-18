using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;

using LugaStore.Application.Common.Exceptions;

namespace LugaStore.Application.Features.Cart.Queries;

public record CartItemDto(int Id, int ProductId, string Name, decimal Price, string Size, int Quantity, decimal Subtotal);

public record GetCartQuery() : IQuery<List<CartItemDto>>;

public class GetCartHandler(IApplicationDbContext context, ICurrentUser currentUser) : IQueryHandler<GetCartQuery, List<CartItemDto>>
{
    public async Task<List<CartItemDto>> Handle(GetCartQuery request, CancellationToken ct)
    {
        var userId = currentUser.Id!.Value;
        return await context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .Select(c => new CartItemDto(
                c.Id,
                c.ProductId,
                c.Product.Name,
                c.Product.Price,
                c.Size.ToString(),
                c.Quantity,
                c.Product.Price * c.Quantity))
            .ToListAsync(ct);
    }
}
