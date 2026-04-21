using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;

namespace SedaWears.Application.Features.Products.Commands;

public record DeleteProductCommand(int Id) : IRequest<Unit>;

public class DeleteProductHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await dbContext.Products.FindAsync([request.Id], ct) ?? throw new NotFoundException("Product not found");
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
