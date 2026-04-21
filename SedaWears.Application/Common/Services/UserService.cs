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
        bool? isActive = null,
        bool? isInvited = false,
        CancellationToken ct = default) where T : BaseUserRepresentation
    {
        var query = dbContext.Users
            .Include(u => u.CreatedBy)
            .Where(u => u.Role == role && u.Id != currentUser.Id);

        if (isActive.HasValue)
            query = query.Where(u => u.IsActive == isActive.Value);

        if (isInvited.HasValue)
            query = query.Where(u => u.EmailConfirmed == !isInvited.Value);

        if (role == UserRole.Customer)
            query = query.Include(u => u.Addresses);

        if (role == UserRole.Manager)
            query = query.Include(u => u.ManagedShops).ThenInclude(ms => ms.Shop);

        query = query.AsNoTracking().OrderByDescending(u => u.CreatedAt);

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
        bool? isActive = null,
        bool? isInvited = null,
        CancellationToken ct = default)
    {
        var query = dbContext.ShopManagers
            .AsNoTracking()
            .Where(sm => sm.ShopId == shopId && sm.ManagerId != currentUser.Id)
            .Include(sm => sm.Manager)
            .ThenInclude(m => m.ManagedShops)
            .ThenInclude(ms => ms.Shop)
            .Include(sm => sm.Manager.CreatedBy)
            .OrderByDescending(sm => sm.CreatedAt)
            .AsQueryable();

        if (isActive.HasValue)
            query = query.Where(sm => sm.Manager.IsActive == isActive.Value);

        var invitedFilter = isInvited ?? false;
        query = query.Where(sm => sm.Manager.EmailConfirmed == !invitedFilter);

        var totalCount = await query.CountAsync(ct);
        var sms = await query.Skip((pageNumber - 1) * pageSize)
                             .Take(pageSize)
                             .ToListAsync(ct);

        var mappedList = sms.Select(sm => (ManagerRepresentation)sm.Manager.ToUserRepresentation()).ToList();

        return new PaginatedList<ManagerRepresentation>(mappedList, totalCount, pageNumber, pageSize);
    }
}
