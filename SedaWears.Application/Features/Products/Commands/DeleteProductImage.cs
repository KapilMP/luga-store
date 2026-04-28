using MediatR;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Products.Commands;

public record DeleteProductImageCommand(int ProductId, int ImageId) : IRequest<Unit>;

public class DeleteProductImageHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteProductImageCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductImageCommand request, CancellationToken ct)
    {
        var image = await dbContext.ProductImages.FindAsync([request.ImageId], ct)
            ?? throw new NotFoundException("Image not found");

        if (image.ProductId != request.ProductId)
            throw new NotFoundException("Image not found");

        dbContext.ProductImages.Remove(image);
        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
