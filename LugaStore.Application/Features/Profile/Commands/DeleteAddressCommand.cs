using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Profile.Commands;

public record DeleteAddressCommand(int UserId, int AddressId) : IRequest;

public class DeleteAddressCommandHandler(IApplicationDbContext dbContext) : 
    IRequestHandler<DeleteAddressCommand>
{
    public async Task Handle(DeleteAddressCommand request, CancellationToken cancellationToken)
    {
        var address = await dbContext.Addresses
            .FirstOrDefaultAsync(a => a.UserId == request.UserId && a.Id == request.AddressId, cancellationToken) 
            ?? throw new NotFoundError("Address not found.");

        dbContext.Addresses.Remove(address);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
