using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Cart.Commands;

public record ClearCartCommand(int UserId) : ICommand;

public class ClearCartHandler(IApplicationDbContext context) : ICommandHandler<ClearCartCommand>
{
    public async Task Handle(ClearCartCommand request, CancellationToken ct)
    {
        var items = context.CartItems.Where(c => c.UserId == request.UserId);
        context.CartItems.RemoveRange(items);
        await context.SaveChangesAsync(ct);
    }
}
