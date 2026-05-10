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
    : IRequest<PaginatedList<CustomerRepresentation>>, IPaginatedQuery;

public class GetCustomersValidator : PaginatedQueryValidator<GetCustomersQuery> { }

public class GetCustomersHandler(IUserService userService) : IRequestHandler<GetCustomersQuery, PaginatedList<CustomerRepresentation>>
{
    public async Task<PaginatedList<CustomerRepresentation>> Handle(GetCustomersQuery request, CancellationToken ct)
    {
        return await userService.GetUsersByRoleAsync<CustomerRepresentation>(
            UserRole.Customer,
            request.PageNumber,
            request.PageSize,
            request.IsInvited,
            request.SortBy,
            request.SortOrder, ct);
    }
}
