using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Features.Shops.Models;
using SedaWears.Application.Features.Shops.Projections;

namespace SedaWears.Application.Features.Shops.Queries;

public record GetShopQuery(int Id) : IRequest<ShopRepresentation>;

public class GetShopHandler(IApplicationDbContext dbContext) : IRequestHandler<GetShopQuery, ShopRepresentation>
{
    public async Task<ShopRepresentation> Handle(GetShopQuery request, CancellationToken ct)
    {
        return await dbContext.Shops
            .AsNoTracking()
            .Where(s => s.Id == request.Id)
            .ProjectToShop()
            .FirstOrDefaultAsync(ct) ?? throw new NotFoundException("Shop not found");
    }
}
