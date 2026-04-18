using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Profile.Commands;

public record DeleteAddressCommand(int AddressId) : IRequest;

public class DeleteAddressCommandHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : 
    IRequestHandler<DeleteAddressCommand>
{
    public async Task Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var address = await dbContext.Addresses
            .FirstOrDefaultAsync(a => a.Id == request.AddressId && a.UserId == userId, cancellationToken)
            ?? throw new NotFoundError("Address not found.");

        dbContext.Addresses.Remove(address);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
