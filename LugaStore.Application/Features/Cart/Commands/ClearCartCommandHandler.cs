using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Cart.Commands;

public class ClearCartCommandHandler(IApplicationDbContext context) : ICommandHandler<ClearCartCommand>
{
    public async Task Handle(ClearCartCommand request, CancellationToken ct)
    {
        var items = context.CartItems.Where(c => c.UserId == request.UserId);
        context.CartItems.RemoveRange(items);
        await context.SaveChangesAsync(ct);
    }
}
