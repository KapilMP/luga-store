using MediatR;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users.Queries;

public record GetOwnerQuery(int Id) : IRequest<OwnerDto>;

public class GetOwnerHandler(IUserService userService) :
    IRequestHandler<GetOwnerQuery, OwnerDto>
{
    public async Task<OwnerDto> Handle(GetOwnerQuery request, CancellationToken ct)
    {
        return await userService.GetUserByIdAndRoleAsync<OwnerDto>(
            request.Id,
            UserRole.Owner,
            ct);
    }
}
