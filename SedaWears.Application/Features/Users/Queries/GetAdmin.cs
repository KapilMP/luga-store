using SedaWears.Application.Features.Users.Models;
using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users.Queries;

public record GetAdminQuery(int Id) : IRequest<AdminRepresentation>;

public class GetAdminHandler(IUserService userService) :
    IRequestHandler<GetAdminQuery, AdminRepresentation>
{
    public async Task<AdminRepresentation> Handle(GetAdminQuery request, CancellationToken ct)
    {
        return await userService.GetUserByIdAndRoleAsync<AdminRepresentation>(
            request.Id, 
            UserRole.Admin, 
            ct);
    }
}
