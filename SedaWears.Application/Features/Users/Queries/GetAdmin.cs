using SedaWears.Application.Features.Users.Models;
using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users.Queries;

public record GetAdminQuery(int Id) : IRequest<AdminDto>;

public class GetAdminHandler(IUserService userService) :
    IRequestHandler<GetAdminQuery, AdminDto>
{
    public async Task<AdminDto> Handle(GetAdminQuery request, CancellationToken ct)
    {
        return await userService.GetUserByIdAndRoleAsync<AdminDto>(
            request.Id, 
            UserRole.Admin, 
            ct);
    }
}
