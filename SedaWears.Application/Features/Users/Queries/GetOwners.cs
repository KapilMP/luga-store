using SedaWears.Application.Features.Users.Models;
using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Models;

using SedaWears.Application.Common.Validators;

namespace SedaWears.Application.Features.Users.Queries;

public record GetOwnersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    bool? IsInvited = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc")
    : IRequest<PaginatedList<OwnerDto>>, IPaginatedQuery;

public class GetOwnersValidator : PaginatedQueryValidator<GetOwnersQuery> { }

public class GetOwnersHandler(IUserService userService) : IRequestHandler<GetOwnersQuery, PaginatedList<OwnerDto>>
{
    public async Task<PaginatedList<OwnerDto>> Handle(GetOwnersQuery request, CancellationToken ct)
    {
        return await userService.GetUsersByRoleAsync<OwnerDto>(
            UserRole.Owner,
            request.PageNumber,
            request.PageSize,
            request.IsInvited,
            request.SortBy,
            request.SortOrder, ct);
    }
}
