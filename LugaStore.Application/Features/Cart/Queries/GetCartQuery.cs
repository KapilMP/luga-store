using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Cart.Queries;

public record CartItemDto(int Id, int ProductId, string Name, decimal Price, string Size, int Quantity, decimal Subtotal);

public record GetCartQuery(int UserId) : IRequest<List<CartItemDto>>;

public class GetCartQueryHandler(IApplicationDbContext context) : IRequestHandler<GetCartQuery, List<CartItemDto>>
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
