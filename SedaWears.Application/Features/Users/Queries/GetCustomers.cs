using SedaWears.Application.Features.Users.Models;
using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Models;

using SedaWears.Application.Common.Validators;

namespace SedaWears.Application.Features.Users.Queries;

public record GetCustomersQuery(
    int PageNumber = 1,
    int PageSize = 10,
    bool? IsInvited = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc")
    : IRequest<PaginatedList<CustomerDto>>, IPaginatedQuery;

public class GetCustomersValidator : PaginatedQueryValidator<GetCustomersQuery> { }

public class GetCustomersHandler(IUserService userService) : IRequestHandler<GetCustomersQuery, PaginatedList<CustomerDto>>
{
    public async Task<PaginatedList<CustomerDto>> Handle(GetCustomersQuery request, CancellationToken ct)
    {
        return await userService.GetUsersByRoleAsync<CustomerDto>(
            UserRole.Customer,
            request.PageNumber,
            request.PageSize,
            request.IsInvited,
            request.SortBy,
            request.SortOrder, ct);
    }
}
