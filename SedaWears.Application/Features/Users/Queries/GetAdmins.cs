using SedaWears.Application.Features.Users.Models;
using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Models;

namespace SedaWears.Application.Features.Users.Queries;

public record GetAdminsQuery(int PageNumber = 1, int PageSize = 10, bool? IsActive = null, bool? IsInvited = null)
    : IRequest<PaginatedList<AdminRepresentation>>;

public class GetAdminsHandler(IUserService userService) : IRequestHandler<GetAdminsQuery, PaginatedList<AdminRepresentation>>
{
    public async Task<PaginatedList<AdminRepresentation>> Handle(GetAdminsQuery request, CancellationToken ct)
    {
        return await userService.GetUsersByRoleAsync<AdminRepresentation>(
            UserRole.Admin,
            request.PageNumber,
            request.PageSize,
            request.IsActive,
            request.IsInvited, ct);
    }
}
