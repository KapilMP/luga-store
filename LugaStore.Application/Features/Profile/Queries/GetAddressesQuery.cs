using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Profile.Queries;

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
