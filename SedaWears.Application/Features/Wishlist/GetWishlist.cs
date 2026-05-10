using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Wishlist;

public record WishlistRepresentation(
    int ProductId, 
    string ProductName, 
    decimal ProductPrice, 
    string? ProductImage, 
    DateTime AddedAt);

public record GetWishlistQuery() : IRequest<List<WishlistRepresentation>>;

public class GetWishlistHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<GetWishlistQuery, List<WishlistRepresentation>>
{
    public async Task<List<WishlistRepresentation>> Handle(GetWishlistQuery request, CancellationToken ct)
    {
        var userId = currentUser.Id ?? throw new UnauthorizedAccessException();

        return await dbContext.WishlistItems
            .AsNoTracking()
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .Select(w => new WishlistRepresentation(
                w.ProductId,
                w.Product.Name,
                w.Product.Price,
                w.Product.Images.OrderBy(i => i.Order).Select(i => i.FileName).FirstOrDefault(),
                w.CreatedAt))
            .ToListAsync(ct);
    }
}
