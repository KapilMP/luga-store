using SedaWears.Application.Features.Users.Models;
using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Models;

using SedaWears.Application.Common.Validators;

namespace SedaWears.Application.Features.Users.Queries;

public record GetAdminsQuery(
    int PageNumber = 1,
    int PageSize = 10,
    bool? IsInvited = null,
    string? SortBy = null,
    string? SortOrder = "desc")
    : IRequest<PaginatedList<AdminDto>>, IPaginatedQuery;

public class GetAdminsValidator : PaginatedQueryValidator<GetAdminsQuery> { }

public class GetAdminsHandler(IUserService userService) : IRequestHandler<GetAdminsQuery, PaginatedList<AdminDto>>
{
    public async Task<PaginatedList<AdminDto>> Handle(GetAdminsQuery request, CancellationToken ct)
    {
        return await userService.GetUsersByRoleAsync<AdminDto>(
            UserRole.Admin,
            request.PageNumber,
            request.PageSize,
            request.IsInvited,
            request.SortBy,
            request.SortOrder, ct);
    }
}
