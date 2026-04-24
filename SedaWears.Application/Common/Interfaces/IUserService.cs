using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Common.Interfaces;

public interface IUserService
{
    Task<PaginatedList<T>> GetUsersByRoleAsync<T>(
        UserRole role,
        int pageNumber,
        int pageSize,
        bool? isInvited = null,
        string? sortBy = null,
        string? sortOrder = "desc",
        CancellationToken ct = default) where T : BaseUserRepresentation;

    Task<PaginatedList<ManagerRepresentation>> GetShopManagersAsync(
        int shopId,
        int pageNumber,
        int pageSize,
        bool? isInvited = null,
        string? sortBy = null,
        string? sortOrder = "desc",
        CancellationToken ct = default);
}
