using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Features.Users;
using Microsoft.AspNetCore.Identity;
using SedaWears.Domain.Entities;
using SedaWears.Application.Common.Settings;
using System.Web;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Features.Users.Projections;

namespace SedaWears.Application.Common.Services;

public class UserService(
    IApplicationDbContext dbContext,
    ICurrentUser currentUser,
    UserManager<User> userManager,
    IEmailService emailService,
    AppConfig appConfig) : IUserService
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

        if (role == UserRole.Manager || role == UserRole.Owner)
            query = query.Include(u => u.ShopMemberships).ThenInclude(sm => sm.Shop);

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
        
        var projectedQuery = role switch
        {
            UserRole.Admin => query.ProjectToAdmin().Cast<T>(),
            UserRole.Owner => query.ProjectToOwner().Cast<T>(),
            UserRole.Manager => query.ProjectToManager().Cast<T>(),
            UserRole.Customer => query.ProjectToCustomer().Cast<T>(),
            _ => throw new ArgumentException("Invalid role")
        };

        var mappedItems = await projectedQuery
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

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
        var query = dbContext.ShopMembers
            .AsNoTracking()
            .Where(sm => sm.ShopId == shopId && sm.UserId != currentUser.Id && sm.User.Role == UserRole.Manager)
            .Include(sm => sm.User)
            .ThenInclude(u => u.ShopMemberships)
            .ThenInclude(sm => sm.Shop)
            .AsQueryable();

        if (isInvited.HasValue)
            query = query.Where(sm => sm.User.EmailConfirmed == !isInvited.Value);

        if (!string.IsNullOrEmpty(sortBy))
        {
            var isDescending = sortOrder?.ToLower() == "desc";
            query = sortBy.ToLower() switch
            {
                "name" => isDescending ? query.OrderByDescending(sm => sm.User.FirstName).ThenByDescending(sm => sm.User.LastName) : query.OrderBy(sm => sm.User.FirstName).ThenBy(sm => sm.User.LastName),
                "email" => isDescending ? query.OrderByDescending(sm => sm.User.Email) : query.OrderBy(sm => sm.User.Email),
                "isactive" => isDescending ? query.OrderByDescending(sm => sm.User.IsActive) : query.OrderBy(sm => sm.User.IsActive),
                "createdat" => isDescending ? query.OrderByDescending(sm => sm.User.CreatedAt) : query.OrderBy(sm => sm.User.CreatedAt),
                _ => query.OrderByDescending(sm => sm.User.CreatedAt)
            };
        }
        else
        {
            query = query.OrderByDescending(sm => sm.User.CreatedAt);
        }

        var totalCount = await query.CountAsync(ct);
        var mappedList = await query
            .Select(sm => sm.User)
            .ProjectToManager()
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

        return new PaginatedList<ManagerRepresentation>(mappedList, totalCount, pageNumber, pageSize);
    }

    public async Task<T> GetUserByIdAndRoleAsync<T>(int userId, UserRole role, CancellationToken ct = default) where T : BaseUserRepresentation
    {
        var query = dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == userId && u.Role == role);

        var projectedQuery = role switch
        {
            UserRole.Admin => query.ProjectToAdmin().Cast<T>(),
            UserRole.Owner => query.ProjectToOwner().Cast<T>(),
            UserRole.Manager => query.ProjectToManager().Cast<T>(),
            UserRole.Customer => query.ProjectToCustomer().Cast<T>(),
            _ => throw new ArgumentException("Invalid role")
        };

        return await projectedQuery.FirstOrDefaultAsync(ct) ?? throw new NotFoundException("User not found.");
    }

    public async Task SendInvitationEmailAsync(User user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}";

        var roleDisplayName = user.Role.ToString();

        var subject = $"SedaWears {roleDisplayName} Invitation";
        var body = $"<p>You have been invited as a <b>{roleDisplayName}</b> to the SedaWears platform.</p>" +
                   $"<p>Click <a href='{url}'>here</a> to accept the invitation and set up your account password.</p>";

        await emailService.SendEmailAsync(user.Email!, subject, body);
    }
}
