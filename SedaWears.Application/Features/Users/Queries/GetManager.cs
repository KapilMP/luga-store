using MediatR;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users.Queries;

public record GetManagerQuery(int Id) : IRequest<ManagerRepresentation>;

public class GetManagerHandler(IUserService userService) :
    IRequestHandler<GetManagerQuery, ManagerRepresentation>
{
    public async Task<ManagerRepresentation> Handle(GetManagerQuery request, CancellationToken ct)
    {
        return await userService.GetUserByIdAndRoleAsync<ManagerRepresentation>(
            request.Id,
            UserRole.Manager,
            ct);
    }
}
