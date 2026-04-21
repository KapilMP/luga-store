using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;

namespace SedaWears.Application.Features.Cart.Commands;

public record RemoveCartItemCommand(int ItemId) : ICommand;

public class RemoveCartItemHandler(IApplicationDbContext context, ICurrentUser currentUser) : ICommandHandler<RemoveCartItemCommand>
{
    public async Task Handle(RemoveCartItemCommand request, CancellationToken ct)
    {
        var userId = currentUser.Id!.Value;
        var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == request.ItemId && c.UserId == userId, ct)
            ?? throw new NotFoundException("Cart item not found");

        context.CartItems.Remove(item);
        await context.SaveChangesAsync(ct);
    }
}
