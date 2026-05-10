using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using Microsoft.EntityFrameworkCore;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Shops.Commands;

public record UpdateShopActiveStatusCommand(int Id, bool IsActive) : IRequest;

public class UpdateShopActiveStatusHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateShopActiveStatusCommand>
{
    public async Task Handle(UpdateShopActiveStatusCommand request, CancellationToken ct)
    {
        var shop = await dbContext.Shops
      .FirstOrDefaultAsync(s => s.Id == request.Id, ct) ?? throw new NotFoundException("Shop not found");

        shop.IsActive = request.IsActive;

        await dbContext.SaveChangesAsync(ct);
    }
}
