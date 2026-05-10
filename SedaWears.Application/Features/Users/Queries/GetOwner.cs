using MediatR;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users.Queries;

public record GetOwnerQuery(int Id) : IRequest<OwnerRepresentation>;

public class GetOwnerHandler(IUserService userService) :
    IRequestHandler<GetOwnerQuery, OwnerRepresentation>
{
    public async Task<OwnerRepresentation> Handle(GetOwnerQuery request, CancellationToken ct)
    {
        return await userService.GetUserByIdAndRoleAsync<OwnerRepresentation>(
            request.Id,
            UserRole.Owner,
            ct);
    }
}
