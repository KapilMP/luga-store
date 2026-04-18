using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Features.Cart.Commands;

public record UpdateCartItemCommand(int ItemId, ProductSize Size, int Quantity) : ICommand;

public class UpdateCartItemHandler(IApplicationDbContext context, ICurrentUser currentUser) : ICommandHandler<UpdateCartItemCommand>
{
    public async Task Handle(UpdateCartItemCommand request, CancellationToken ct)
    {
        var userId = currentUser.Id!.Value;
        var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == request.ItemId && c.UserId == userId, ct) 
            ?? throw new NotFoundError("Cart item not found");
        item.Size = request.Size;
        item.Quantity = request.Quantity;
        await context.SaveChangesAsync(ct);
    }
}
