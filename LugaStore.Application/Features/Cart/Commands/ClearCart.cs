using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Cart.Commands;

public record ClearCartCommand() : ICommand;

public class ClearCartHandler(IApplicationDbContext context, ICurrentUser currentUser) : ICommandHandler<ClearCartCommand>
{
    public async Task Handle(ClearCartCommand request, CancellationToken ct)
    {
        var userId = currentUser.Id!.Value;
        var items = context.CartItems.Where(c => c.UserId == userId);
        context.CartItems.RemoveRange(items);
        await context.SaveChangesAsync(ct);
    }
}
