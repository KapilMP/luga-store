using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Profile.Commands;

public record AddAddressCommand(AddressRepresentation Address) : IRequest<AddressRepresentation>;

public class AddAddressCommandHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : 
    IRequestHandler<AddAddressCommand, AddressRepresentation>
{
    public async Task<AddressRepresentation> Handle(AddAddressCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var user = await dbContext.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == userId, cancellationToken) ?? throw new NotFoundException("User not found.");

        var entity = new Address
        {
            UserId = userId,
            Label = request.Address.Label,
            FullName = request.Address.FullName,
            Email = request.Address.Email,
            Phone = request.Address.Phone,
            Street = request.Address.Address,
            City = request.Address.City,
            ZipCode = request.Address.ZipCode
        };

        user.Addresses.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddressRepresentation(entity.Id, entity.Label, entity.FullName, entity.Email, entity.Phone, entity.Street, entity.City, entity.ZipCode);
    }
}
