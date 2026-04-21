using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Features.Shops.Models;

namespace SedaWears.Application.Features.Shops.Queries;

public record GetShopQuery(int Id) : IRequest<ShopRepresentation>;

public class GetShopHandler(IApplicationDbContext dbContext) : IRequestHandler<GetShopQuery, ShopRepresentation>
{
    public async Task<ShopRepresentation> Handle(GetShopQuery request, CancellationToken ct)
    {
        var shop = await dbContext.Shops
            .Include(s => s.Owners)
            .ThenInclude(o => o.Owner)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.Id, ct) ?? throw new NotFoundException("Shop not found");

        return new ShopRepresentation(
            shop.Id,
            shop.Name,
            shop.Slug,
            shop.Description,
            shop.LogoFileName,
            shop.IsActive,
            shop.CreatedAt,
            shop.Owners.Select(o => new ShopOwnerSummary(o.OwnerId, o.Owner.FirstName ?? string.Empty, o.Owner.LastName ?? string.Empty, o.Owner.Email ?? string.Empty)).ToList()
        );
    }
}
