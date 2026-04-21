using SedaWears.Application.Features.Users.Models;
using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Models;

namespace SedaWears.Application.Features.Users.Queries;

public record GetCustomersQuery(
    int PageNumber = 1, 
    int PageSize = 10, 
    bool? IsActive = null, 
    bool? IsInvited = null,
    string? SortBy = null,
    string? SortOrder = "desc") 
    : IRequest<PaginatedList<CustomerRepresentation>>;

public class GetCustomersHandler(IUserService userService) : IRequestHandler<GetCustomersQuery, PaginatedList<CustomerRepresentation>>
{
    public async Task<PaginatedList<CustomerRepresentation>> Handle(GetCustomersQuery request, CancellationToken ct)
    {
        return await userService.GetUsersByRoleAsync<CustomerRepresentation>(
            UserRole.Customer, 
            request.PageNumber, 
            request.PageSize, 
            request.IsActive, 
            request.IsInvited,
            request.SortBy,
            request.SortOrder, ct);
    }
}
