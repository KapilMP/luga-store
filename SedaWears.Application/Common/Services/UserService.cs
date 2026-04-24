using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Features.Users;

namespace SedaWears.Application.Common.Services;

public class UserService(IApplicationDbContext dbContext, ICurrentUser currentUser) : IUserService
{
    public async Task<PaginatedList<T>> GetUsersByRoleAsync<T>(
        UserRole role,
        int pageNumber,
        int pageSize,
        bool? isInvited = null,
        string? sortBy = null,
        string? sortOrder = "desc",
        CancellationToken ct = default) where T : BaseUserRepresentation
    {
        var query = dbContext.Users
            .Where(u => u.Role == role && u.Id != currentUser.Id);

        if (isInvited.HasValue)
            query = query.Where(u => u.EmailConfirmed == !isInvited.Value);

        if (role == UserRole.Customer)
            query = query.Include(u => u.Addresses);

        if (role == UserRole.Manager)
            query = query.Include(u => u.ManagedShops).ThenInclude(ms => ms.Shop);

        query = query.AsNoTracking();

        if (!string.IsNullOrEmpty(sortBy))
        {
            var isDescending = sortOrder?.ToLower() == "desc";
            query = sortBy.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(u => u.FirstName).ThenByDescending(u => u.LastName) : query.OrderBy(u => u.FirstName).ThenBy(u => u.LastName),
                "email" => isDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
                "isactive" => isDescending ? query.OrderByDescending(u => u.IsActive) : query.OrderBy(u => u.IsActive),
                "createdat" => isDescending ? query.OrderByDescending(u => u.CreatedAt) : query.OrderBy(u => u.CreatedAt),
                _ => query.OrderByDescending(u => u.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(u => u.CreatedAt);
        }

        var totalCount = await query.CountAsync(ct);
        var users = await query.Skip((pageNumber - 1) * pageSize)
                               .Take(pageSize)
                               .ToListAsync(ct);

        var mappedItems = users.Select(u => (T)u.ToUserRepresentation()).ToList();

        return new PaginatedList<T>(mappedItems, totalCount, pageNumber, pageSize);
    }

    public async Task<PaginatedList<ManagerRepresentation>> GetShopManagersAsync(
        int shopId,
        int pageNumber,
        int pageSize,
        bool? isInvited = null,
        string? sortBy = null,
        string? sortOrder = "desc",
        CancellationToken ct = default)
    {
        var query = dbContext.ShopManagers
            .AsNoTracking()
            .Where(sm => sm.ShopId == shopId && sm.ManagerId != currentUser.Id)
            .Include(sm => sm.Manager)
            .ThenInclude(m => m.ManagedShops)
            .ThenInclude(ms => ms.Shop)
            .AsQueryable();

        if (isInvited.HasValue)
            query = query.Where(sm => sm.Manager.EmailConfirmed == !isInvited.Value);

        if (!string.IsNullOrEmpty(sortBy))
        {
            var isDescending = sortOrder?.ToLower() == "desc";
            query = sortBy.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(sm => sm.Manager.FirstName).ThenByDescending(sm => sm.Manager.LastName) : query.OrderBy(sm => sm.Manager.FirstName).ThenBy(sm => sm.Manager.LastName),
                "email" => isDescending ? query.OrderByDescending(sm => sm.Manager.Email) : query.OrderBy(sm => sm.Manager.Email),
                "isactive" => isDescending ? query.OrderByDescending(sm => sm.Manager.IsActive) : query.OrderBy(sm => sm.Manager.IsActive),
                "createdat" => isDescending ? query.OrderByDescending(sm => sm.Manager.CreatedAt) : query.OrderBy(sm => sm.Manager.CreatedAt),
                _ => query.OrderByDescending(sm => sm.Manager.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(sm => sm.Manager.CreatedAt);
        }

        var totalCount = await query.CountAsync(ct);
        var sms = await query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync(ct);

        var mappedList = sms.Select(sm => (ManagerRepresentation)sm.Manager.ToUserRepresentation()).ToList();

        return new PaginatedList<ManagerRepresentation>(mappedList, totalCount, pageNumber, pageSize);
    }
}
