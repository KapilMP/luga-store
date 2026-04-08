using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Commands;

public record AddAddressCommand(int UserId, AddressRepresentation Address) : IRequest<AddressRepresentation>;

public class AddAddressCommandHandler(IApplicationDbContext dbContext) : 
    IRequestHandler<AddAddressCommand, AddressRepresentation>
{
    public async Task<AddressRepresentation> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken) ?? throw new NotFoundError("User not found.");

        var entity = new Address
        {
            UserId = request.UserId,
            Label = request.Address.Label,
            FullName = request.Address.FullName,
            Email = request.Address.Email,
            Phone = request.Address.Phone,
            Street = request.Address.Street,
            City = request.Address.City,
            ZipCode = request.Address.ZipCode
        };

        user.Addresses.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddressRepresentation(entity.Id, entity.Label, entity.FullName, entity.Email, entity.Phone, entity.Street, entity.City, entity.ZipCode);
    }
}
