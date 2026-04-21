using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Profile.Queries;

public record GetAddressesQuery() : IRequest<List<AddressRepresentation>>;

public class GetAddressesQueryHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : 
    IRequestHandler<GetAddressesQuery, List<AddressRepresentation>>
{
    public async Task<List<AddressRepresentation>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var addresses = await dbContext.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

        return addresses.Select(a => new AddressRepresentation(
            a.Id, a.Label, a.FullName, a.Email, a.Phone, a.Street, a.City, a.ZipCode)).ToList();
    }
}
