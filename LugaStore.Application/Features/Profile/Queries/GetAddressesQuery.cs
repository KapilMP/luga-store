using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Features.Users.Models;

namespace LugaStore.Application.Features.Profile.Queries;

public record GetAddressesQuery(int UserId) : IRequest<List<AddressRepresentation>>;

public class GetAddressesQueryHandler(IApplicationDbContext dbContext) : 
    IRequestHandler<GetAddressesQuery, List<AddressRepresentation>>
{
    public async Task<List<AddressRepresentation>> Handle(GetAddressesQuery request, CancellationToken cancellationToken)
    {
        var addresses = await dbContext.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == request.UserId)
            .ToListAsync(cancellationToken);

        return addresses.Select(a => new AddressRepresentation(
            a.Id, a.Label, a.FullName, a.Email, a.Phone, a.Street, a.City, a.ZipCode)).ToList();
    }
}
