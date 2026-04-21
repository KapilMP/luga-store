using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;

using SedaWears.Application.Common.Exceptions;

using SedaWears.Application.Features.Cart.Models;

namespace SedaWears.Application.Features.Cart.Queries;

public record GetCartQuery() : IQuery<List<CartItemRepresentation>>;

public class GetCartHandler(IApplicationDbContext context, ICurrentUser currentUser) : IQueryHandler<GetCartQuery, List<CartItemRepresentation>>
{
    public async Task<List<CartItemRepresentation>> Handle(GetCartQuery request, CancellationToken ct)
    {
        var userId = currentUser.Id;
        return await context.CartItems
            .Where(c => c.UserId == userId)
            .Include(c => c.Product)
            .Select(c => new CartItemRepresentation(
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
