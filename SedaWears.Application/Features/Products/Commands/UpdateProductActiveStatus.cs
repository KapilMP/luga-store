using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;

namespace SedaWears.Application.Features.Products.Commands;

public record UpdateProductActiveStatusCommand(int Id, bool IsActive, int? ShopId = null) : IRequest;

public class UpdateProductActiveStatusHandler(IApplicationDbContext dbContext) : IRequestHandler<UpdateProductActiveStatusCommand>
{
    public async Task Handle(UpdateProductActiveStatusCommand request, CancellationToken ct)
    {
        var product = await dbContext.Products
            .Include(p => p.Category)
            .FirstOrDefaultAsync(p => p.Id == request.Id, ct) ?? throw new NotFoundException("Product not found");

        if (request.ShopId.HasValue && product.Category.ShopId != request.ShopId)
        {
            throw new NotFoundException("Product not found.");
        }

        product.IsActive = request.IsActive;
        await dbContext.SaveChangesAsync(ct);
    }
}
