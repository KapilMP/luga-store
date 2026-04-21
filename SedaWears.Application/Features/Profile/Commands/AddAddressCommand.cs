using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Profile.Commands;

public record AddAddressCommand(
    string Label,
    string FullName,
    string Email,
    string Phone,
    string Street,
    string City,
    string ZipCode
) : IRequest<AddressRepresentation>;

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
            Label = request.Label,
            FullName = request.FullName,
            Email = request.Email,
            Phone = request.Phone,
            Street = request.Street,
            City = request.City,
            ZipCode = request.ZipCode
        };

        user.Addresses.Add(entity);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddressRepresentation(entity.Id, entity.Label, entity.FullName, entity.Email, entity.Phone, entity.Street, entity.City, entity.ZipCode);
    }
}
