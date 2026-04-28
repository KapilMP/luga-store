using MediatR;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Products.Commands;

public record AddProductImageCommand(int ProductId, string FileName) : IRequest<int>;

public class AddProductImageHandler(IApplicationDbContext dbContext) : IRequestHandler<AddProductImageCommand, int>
{
    public async Task<int> Handle(AddProductImageCommand request, CancellationToken ct)
    {
        var product = await dbContext.Products.FindAsync([request.ProductId], ct)
            ?? throw new NotFoundException("Product not found");

        var maxOrder = dbContext.ProductImages
            .Where(i => i.ProductId == request.ProductId)
            .Select(i => (int?)i.Order)
            .Max() ?? -1;

        var image = new ProductImage
        {
            ProductId = request.ProductId,
            FileName = request.FileName,
            Order = maxOrder + 1
        };

        dbContext.ProductImages.Add(image);
        await dbContext.SaveChangesAsync(ct);
        return image.Id;
    }
}
