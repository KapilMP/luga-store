using MediatR;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users.Queries;

public record GetManagerQuery(int Id) : IRequest<ManagerDto>;

public class GetManagerHandler(IUserService userService) :
    IRequestHandler<GetManagerQuery, ManagerDto>
{
    public async Task<ManagerDto> Handle(GetManagerQuery request, CancellationToken ct)
    {
        return await userService.GetUserByIdAndRoleAsync<ManagerDto>(
            request.Id,
            UserRole.Manager,
            ct);
    }
}
