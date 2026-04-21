using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Cart.Commands;

public record ClearCartCommand() : ICommand;

public class ClearCartHandler(IApplicationDbContext context, ICurrentUser currentUser) : ICommandHandler<ClearCartCommand>
{
    public async Task Handle(ClearCartCommand request, CancellationToken ct)
    {
        var userId = currentUser.Id;
        var items = context.CartItems.Where(c => c.UserId == userId);
        context.CartItems.RemoveRange(items);
        await context.SaveChangesAsync(ct);
    }
}
