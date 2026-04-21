using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Cart.Commands;

public record UpdateCartItemCommand(int ItemId, ProductSize Size, int Quantity) : ICommand;

public class UpdateCartItemHandler(IApplicationDbContext context, ICurrentUser currentUser) : ICommandHandler<UpdateCartItemCommand>
{
    public async Task Handle(UpdateCartItemCommand request, CancellationToken ct)
    {
        var userId = currentUser.Id!.Value;
        var item = await context.CartItems.FirstOrDefaultAsync(c => c.Id == request.ItemId && c.UserId == userId, ct) 
            ?? throw new NotFoundException("Cart item not found");
        item.Size = request.Size;
        item.Quantity = request.Quantity;
        await context.SaveChangesAsync(ct);
    }
}
