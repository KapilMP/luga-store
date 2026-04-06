using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;

namespace LugaStore.Application.Features.Cart.Commands;

public class RemoveCartItemCommandHandler(IApplicationDbContext context) : ICommandHandler<RemoveCartItemCommand>
{
    public async Task Handle(RemoveCartItemCommand request, CancellationToken ct)
    {
        var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == request.ItemId && c.UserId == request.UserId, ct);
        if (item == null) throw new NotFoundError("Cart item not found");

        context.CartItems.Remove(item);
        await context.SaveChangesAsync(ct);
    }
}
