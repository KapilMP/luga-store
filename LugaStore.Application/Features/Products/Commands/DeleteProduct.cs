using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;

namespace LugaStore.Application.Features.Products.Commands;

public record DeleteProductCommand(int Id) : IRequest<Unit>;

public class DeleteProductHandler(IApplicationDbContext dbContext) : IRequestHandler<DeleteProductCommand, Unit>
{
    public async Task<Unit> Handle(DeleteProductCommand request, CancellationToken ct)
    {
        var product = await dbContext.Products.FindAsync([request.Id], ct) ?? throw new NotFoundError("Product not found");
        dbContext.Products.Remove(product);
        await dbContext.SaveChangesAsync(ct);
        return Unit.Value;
    }
}
