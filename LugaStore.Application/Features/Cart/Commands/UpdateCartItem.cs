using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Enums;

namespace LugaStore.Application.Features.Cart.Commands;

public record UpdateCartItemCommand(int ItemId, int UserId, ProductSize Size, int Quantity) : ICommand;

public class UpdateCartItemHandler(IApplicationDbContext context) : ICommandHandler<UpdateCartItemCommand>
{
    public async Task Handle(UpdateCartItemCommand request, CancellationToken ct)
    {
        var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == request.ItemId && c.UserId == request.UserId, ct) 
            ?? throw new NotFoundError("Cart item not found");
        item.Size = request.Size;
        item.Quantity = request.Quantity;
        await context.SaveChangesAsync(ct);
    }
}
