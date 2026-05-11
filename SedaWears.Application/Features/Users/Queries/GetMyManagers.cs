using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Users.Models;

using SedaWears.Application.Common.Validators;

namespace SedaWears.Application.Features.Users.Queries;

public record GetMyManagersQuery(int PageNumber = 1, int PageSize = 10, bool? Invited = null, string? SortBy = "createdAt", string? SortOrder = "desc")
    : IRequest<PaginatedList<ManagerDto>>, IPaginatedQuery;

public class GetMyManagersValidator : PaginatedQueryValidator<GetMyManagersQuery> { }

public class GetMyManagersHandler(IUserService userService, ICurrentUser currentUser) : IRequestHandler<GetMyManagersQuery, PaginatedList<ManagerDto>>
{
    public async Task<PaginatedList<ManagerDto>> Handle(GetMyManagersQuery request, CancellationToken ct)
    {
        var shopId = currentUser.ShopId ?? throw new UnauthorizedAccessException("Shop context missing. Please provide X-Shop-ID header.");

        return await userService.GetShopManagersAsync(
            shopId,
            request.PageNumber,
            request.PageSize,
            request.Invited,
            request.SortBy,
            request.SortOrder, ct);
    }
}
