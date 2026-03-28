using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;
using LugaStore.Infrastructure.Settings;

namespace LugaStore.Infrastructure.Services;

public class PartnerService(
    ICurrentUser currentUser,
    UserManager<User> userManager,
    IEmailSender emailSender,
    IAppSettings appSettings,
    IUserService userService,
    IApplicationDbContext dbContext) : IPartnerService
{
    private string? UserId => currentUser.UserId;

    #region Admin Methods
    public async Task InvitePartnerAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await CreateInvitedUserAsync(email, Roles.Partner, cancellationToken);
        await SendInvitationEmailAsync(user);
    }

    public async Task ResendPartnerInvitationAsync(int partnerId, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserWithRoleAsync(partnerId, Roles.Partner);
        if (user.EmailConfirmed) throw new BadRequestError("User has already accepted the invitation.");
        await SendInvitationEmailAsync(user);
    }

    public async Task InvitePartnerManagerAsync(int partnerId, string email, CancellationToken cancellationToken = default)
    {
        _ = await userService.GetUserWithRoleAsync(partnerId, Roles.Partner);

        var user = await CreateInvitedUserAsync(email, Roles.PartnerManager, cancellationToken);

        if (await dbContext.PartnerManagers.AnyAsync(pm => pm.PartnerId == partnerId && pm.ManagerId == user.Id, cancellationToken))
            throw new ConflictError("Manager is already assigned to this partner.");

        dbContext.PartnerManagers.Add(new PartnerManager { PartnerId = partnerId, ManagerId = user.Id });
        await dbContext.SaveChangesAsync(cancellationToken);
        await SendInvitationEmailAsync(user);
    }

    public async Task ResendPartnerManagerInvitationAsync(int partnerId, int managerId, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserWithRoleAsync(managerId, Roles.PartnerManager);
        if (user.EmailConfirmed) throw new BadRequestError("User has already accepted the invitation.");

        var hasMapping = await dbContext.PartnerManagers.AnyAsync(pm => pm.PartnerId == partnerId && pm.ManagerId == managerId, cancellationToken);
        if (!hasMapping) throw new NotFoundError("Manager assignment not found.");

        await SendInvitationEmailAsync(user);
    }

    public async Task DeletePartnerManagerAsync(int partnerId, int managerId, CancellationToken cancellationToken = default)
    {
        var mapping = await dbContext.PartnerManagers.FirstOrDefaultAsync(pm => pm.PartnerId == partnerId && pm.ManagerId == managerId, cancellationToken)
            ?? throw new NotFoundError("Manager assignment not found.");

        dbContext.PartnerManagers.Remove(mapping);
        await dbContext.SaveChangesAsync(cancellationToken);

        // If manager has no other partner assignments, we can delete the user record
        var stillAssigned = await dbContext.PartnerManagers.AnyAsync(pm => pm.ManagerId == managerId, cancellationToken);
        if (!stillAssigned)
        {
            var user = await userService.GetUserWithRoleAsync(managerId, Roles.PartnerManager);
            await userManager.DeleteAsync(user);
        }
    }

    public Task ActivatePartnerAsync(int partnerId, CancellationToken cancellationToken = default) => SetActiveStatusAsync(partnerId, Roles.Partner, true);
    public Task DeactivatePartnerAsync(int partnerId, CancellationToken cancellationToken = default) => SetActiveStatusAsync(partnerId, Roles.Partner, false);
    
    public Task ActivatePartnerManagerAsync(int partnerId, int managerId, CancellationToken cancellationToken = default) 
    {
         _ = GetPartnerManagerMappingAsync(partnerId, managerId);
         return SetActiveStatusAsync(managerId, Roles.PartnerManager, true);
    }
    
    public Task DeactivatePartnerManagerAsync(int partnerId, int managerId, CancellationToken cancellationToken = default)
    {
         _ = GetPartnerManagerMappingAsync(partnerId, managerId);
         return SetActiveStatusAsync(managerId, Roles.PartnerManager, false);
    }

    public async Task DeletePartnerAsync(int partnerId, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserWithRoleAsync(partnerId, Roles.Partner);
        await userManager.DeleteAsync(user);
    }

    public async Task<PartnerProfileDto> GetPartnerByPartnerIdAsync(int partnerId)
    {
        return await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == partnerId)
            .Select(u => new PartnerProfileDto
            {
                Id = u.Id,
                FirstName = u.FirstName ?? string.Empty,
                LastName = u.LastName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                AvatarUrl = u.AvatarPath ?? string.Empty,
                Phone = u.PhoneNumber ?? string.Empty
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundError("Partner not found.");
    }

    public async Task<List<PartnerProfileDto>> GetPartnersAsync()
    {
        var roleId = await dbContext.Roles
            .AsNoTracking()
            .Where(r => r.Name == Roles.Partner)
            .Select(r => r.Id)
            .FirstOrDefaultAsync();

        return await dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.RoleId == roleId)
            .Join(dbContext.Users, ur => ur.UserId, u => u.Id, (ur, u) => new PartnerProfileDto
            {
                Id = u.Id,
                FirstName = u.FirstName ?? string.Empty,
                LastName = u.LastName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                AvatarUrl = u.AvatarPath ?? string.Empty,
                Phone = u.PhoneNumber ?? string.Empty
            })
            .ToListAsync();
    }

    public async Task<PartnerManagerProfileDto> GetPartnerManagerByPartnerIdAndManagerIdAsync(int partnerId, int managerId)
    {
        return await dbContext.PartnerManagers
            .AsNoTracking()
            .Where(pm => pm.PartnerId == partnerId && pm.ManagerId == managerId)
            .Select(pm => new PartnerManagerProfileDto
            {
                Id = pm.Manager.Id,
                FirstName = pm.Manager.FirstName ?? string.Empty,
                LastName = pm.Manager.LastName ?? string.Empty,
                Email = pm.Manager.Email ?? string.Empty,
                AvatarUrl = pm.Manager.AvatarPath ?? string.Empty,
                Phone = pm.Manager.PhoneNumber ?? string.Empty
            })
            .FirstOrDefaultAsync() ?? throw new NotFoundError("Manager assignment not found.");
    }

    public async Task<List<PartnerManagerProfileDto>> GetPartnerManagersByPartnerIdAsync(int partnerId)
    {
        return await dbContext.PartnerManagers
            .AsNoTracking()
            .Where(pm => pm.PartnerId == partnerId)
            .Select(pm => new PartnerManagerProfileDto
            {
                Id = pm.Manager.Id,
                FirstName = pm.Manager.FirstName ?? string.Empty,
                LastName = pm.Manager.LastName ?? string.Empty,
                Email = pm.Manager.Email ?? string.Empty,
                AvatarUrl = pm.Manager.AvatarPath ?? string.Empty,
                Phone = pm.Manager.PhoneNumber ?? string.Empty
            })
            .ToListAsync();
    }
    public async Task<List<PartnerProfileDto>> GetInvitedPartnersAsync()
    {
        var roleId = await dbContext.Roles.AsNoTracking().Where(r => r.Name == Roles.Partner).Select(r => r.Id).FirstOrDefaultAsync();

        return await dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.RoleId == roleId)
            .Join(dbContext.Users.Where(u => !u.EmailConfirmed), ur => ur.UserId, u => u.Id, (ur, u) => new PartnerProfileDto
            {
                Id = u.Id,
                FirstName = u.FirstName ?? string.Empty,
                LastName = u.LastName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                AvatarUrl = u.AvatarPath ?? string.Empty,
                Phone = u.PhoneNumber ?? string.Empty
            })
            .ToListAsync();
    }

    public async Task<List<PartnerManagerProfileDto>> GetInvitedPartnerManagersByPartnerIdAsync(int partnerId)
    {
        return await dbContext.PartnerManagers
            .AsNoTracking()
            .Where(pm => pm.PartnerId == partnerId && !pm.Manager.EmailConfirmed)
            .Select(pm => new PartnerManagerProfileDto
            {
                Id = pm.Manager.Id,
                FirstName = pm.Manager.FirstName ?? string.Empty,
                LastName = pm.Manager.LastName ?? string.Empty,
                Email = pm.Manager.Email ?? string.Empty,
                AvatarUrl = pm.Manager.AvatarPath ?? string.Empty,
                Phone = pm.Manager.PhoneNumber ?? string.Empty
            })
            .ToListAsync();
    }
    #endregion

    #region Partner Methods (Uses Current Logged-In Partner Context)
    public async Task<List<PartnerManagerProfileDto>> GetManagersAsync()
    {
        var partnerId = int.Parse(UserId!);
        return await GetPartnerManagersByPartnerIdAsync(partnerId);
    }

    public async Task<PartnerManagerProfileDto> GetManagerByManagerIdAsync(int managerId)
    {
        var partnerId = int.Parse(UserId!);
        return await GetPartnerManagerByPartnerIdAndManagerIdAsync(partnerId, managerId);
    }

    public async Task InviteManagerAsync(string email, CancellationToken cancellationToken = default)
    {
        var partnerId = int.Parse(UserId!);
        await InvitePartnerManagerAsync(partnerId, email, cancellationToken);
    }

    public async Task ResendManagerInvitationAsync(int managerId, CancellationToken cancellationToken = default)
    {
        var partnerId = int.Parse(UserId!);
        await ResendPartnerManagerInvitationAsync(partnerId, managerId, cancellationToken);
    }

    public Task DeleteManagerAsync(int managerId, CancellationToken cancellationToken = default)
    {
        var partnerId = int.Parse(UserId!);
        return DeletePartnerManagerAsync(partnerId, managerId, cancellationToken);
    }

    public Task ActivateManagerAsync(int managerId, CancellationToken cancellationToken = default)
    {
        var partnerId = int.Parse(UserId!);
        return ActivatePartnerManagerAsync(partnerId, managerId, cancellationToken);
    }

    public Task DeactivateManagerAsync(int managerId, CancellationToken cancellationToken = default)
    {
        var partnerId = int.Parse(UserId!);
        return DeactivatePartnerManagerAsync(partnerId, managerId, cancellationToken);
    }
    public async Task<List<PartnerManagerProfileDto>> GetInvitedManagersAsync()
    {
        var partnerId = int.Parse(UserId!);
        return await GetInvitedPartnerManagersByPartnerIdAsync(partnerId);
    }
    #endregion

    #region Helpers


    private async Task<PartnerManager> GetPartnerManagerMappingAsync(int partnerId, int managerId)
    {
        return await dbContext.PartnerManagers.Include(pm => pm.Manager)
            .FirstOrDefaultAsync(pm => pm.PartnerId == partnerId && pm.ManagerId == managerId) 
            ?? throw new NotFoundError("Manager assignment not found.");
    }

    private async Task SetActiveStatusAsync(int userId, string role, bool isActive)
    {
        var user = await userService.GetUserWithRoleAsync(userId, role);
        user.IsActive = isActive;
        await userManager.UpdateAsync(user);
    }

    private async Task<User> CreateInvitedUserAsync(string email, string role, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(email) != null) throw new ConflictError("Email already exists.");

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = new User { Email = email, EmailConfirmed = false };
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded) throw new InternalServerError("Failed to create user.");

            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded) throw new InternalServerError("Failed to assign role.");

            await transaction.CommitAsync(cancellationToken);
            return user;
        }
        catch
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }
    }

    private async Task SendInvitationEmailAsync(User user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var inviteUrl = $"{appSettings.FrontendUrl}/accept-invitation?email={user.Email}&token={encodedToken}";
        await emailSender.SendEmailAsync(user.Email!, "You're invited to Luga Store", $"You've been invited. Set your password here: {inviteUrl}");
    }
    #endregion
}