using SedaWears.Application.Features.Users.Models;
using MediatR;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Models;

namespace SedaWears.Application.Features.Users.Queries;

public record GetShopManagersByShopIdQuery(
    int ShopId,
    int PageNumber = 1,
    int PageSize = 10,
    bool? Invited = null,
    string? SortBy = "createdAt",
    string? SortOrder = "desc")
    : IRequest<PaginatedList<ManagerRepresentation>>;

public class GetShopManagersByShopIdHandler(IUserService userService) : IRequestHandler<GetShopManagersByShopIdQuery, PaginatedList<ManagerRepresentation>>
{
    public async Task<PaginatedList<ManagerRepresentation>> Handle(GetShopManagersByShopIdQuery request, CancellationToken ct)
    {
        return await userService.GetShopManagersAsync(
            request.ShopId,
            request.PageNumber,
            request.PageSize,
            request.Invited,
            request.SortBy,
            request.SortOrder, ct);
    }
}
