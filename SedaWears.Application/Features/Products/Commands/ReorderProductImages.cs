using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Products.Commands;

public record ReorderProductImageItem(int ImageId, int Order);
public record ReorderProductImagesCommand(int ProductId, List<ReorderProductImageItem> Items) : IRequest<Unit>;

public class ReorderProductImagesHandler(IApplicationDbContext dbContext) : IRequestHandler<ReorderProductImagesCommand, Unit>
{
    public async Task<Unit> Handle(ReorderProductImagesCommand request, CancellationToken ct)
    {
        var images = await dbContext.ProductImages
            .Where(i => i.ProductId == request.ProductId)
            .ToListAsync(ct);

        foreach (var item in request.Items)
        {
            var image = images.FirstOrDefault(i => i.Id == item.ImageId)
                ?? throw new NotFoundException($"Image {item.ImageId} not found");
            image.Order = item.Order;
        }

        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
