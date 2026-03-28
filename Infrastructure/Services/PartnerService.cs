using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Infrastructure.Services;

public class PartnerService(
    ICurrentUser currentUser,
    UserManager<User> userManager,
    IUserService userService,
    IApplicationDbContext dbContext) : IPartnerService
{
    private string? UserId => currentUser.UserId;

    #region Admin Methods
    public async Task InvitePartnerAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userService.CreateInvitedUserAsync(email, Roles.Partner, cancellationToken);
        await userService.SendInvitationEmailAsync(user);
    }

    public async Task ResendPartnerInvitationAsync(int partnerId, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserWithRoleAsync(partnerId, Roles.Partner);
        if (user.EmailConfirmed) throw new BadRequestError("User has already accepted the invitation.");
        await userService.SendInvitationEmailAsync(user);
    }

    public async Task InvitePartnerManagerAsync(int partnerId, string email, CancellationToken cancellationToken = default)
    {
        _ = await userService.GetUserWithRoleAsync(partnerId, Roles.Partner);
        var user = await userService.CreateInvitedUserAsync(email, Roles.PartnerManager, cancellationToken);

        if (await dbContext.PartnerManagers.AnyAsync(pm => pm.PartnerId == partnerId && pm.ManagerId == user.Id, cancellationToken))
            throw new ConflictError("Manager is already assigned to this partner.");

        dbContext.PartnerManagers.Add(new PartnerManager { PartnerId = partnerId, ManagerId = user.Id });
        await dbContext.SaveChangesAsync(cancellationToken);
        await userService.SendInvitationEmailAsync(user);
    }

    public async Task ResendPartnerManagerInvitationAsync(int partnerId, int managerId, CancellationToken cancellationToken = default)
    {
        var user = await userService.GetUserWithRoleAsync(managerId, Roles.PartnerManager);
        if (user.EmailConfirmed) throw new BadRequestError("User has already accepted the invitation.");

        var hasMapping = await dbContext.PartnerManagers.AnyAsync(pm => pm.PartnerId == partnerId && pm.ManagerId == managerId, cancellationToken);
        if (!hasMapping) throw new NotFoundError("Manager assignment not found.");

        await userService.SendInvitationEmailAsync(user);
    }

    public async Task DeletePartnerManagerAsync(int partnerId, int managerId, CancellationToken cancellationToken = default)
    {
        var mapping = await dbContext.PartnerManagers.FirstOrDefaultAsync(pm => pm.PartnerId == partnerId && pm.ManagerId == managerId, cancellationToken)
            ?? throw new NotFoundError("Manager assignment not found.");

        dbContext.PartnerManagers.Remove(mapping);
        await dbContext.SaveChangesAsync(cancellationToken);

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
        => await userService.GetUserWithRoleAsync<PartnerProfileDto>(partnerId, Roles.Partner) ?? throw new NotFoundError("Partner not found.");

    public Task<List<PartnerProfileDto>> GetPartnersAsync()
        => userService.GetUsersByRoleAsync<PartnerProfileDto>(Roles.Partner);

    public async Task<PartnerManagerProfileDto> GetPartnerManagerByPartnerIdAndManagerIdAsync(int partnerId, int managerId)
    {
        return await dbContext.PartnerManagers
            .AsNoTracking()
            .Where(pm => pm.PartnerId == partnerId && pm.ManagerId == managerId)
            .Select(pm => PartnerManagerProfileDto.From(pm.Manager))
            .FirstOrDefaultAsync() ?? throw new NotFoundError("Manager assignment not found.");
    }

    public async Task<List<PartnerManagerProfileDto>> GetPartnerManagersByPartnerIdAsync(int partnerId)
    {
        return await dbContext.PartnerManagers
            .AsNoTracking()
            .Where(pm => pm.PartnerId == partnerId)
            .Select(pm => PartnerManagerProfileDto.From(pm.Manager))
            .ToListAsync();
    }
    
    public Task<List<PartnerProfileDto>> GetInvitedPartnersAsync()
        => userService.GetUsersByRoleAsync<PartnerProfileDto>(Roles.Partner, confirmedOnly: false);

    public async Task<List<PartnerManagerProfileDto>> GetInvitedPartnerManagersByPartnerIdAsync(int partnerId)
    {
        return await dbContext.PartnerManagers
            .AsNoTracking()
            .Where(pm => pm.PartnerId == partnerId && !pm.Manager.EmailConfirmed)
            .Select(pm => PartnerManagerProfileDto.From(pm.Manager))
            .ToListAsync();
    }
    #endregion

    #region Partner Methods (Contextual)
    public Task<List<PartnerManagerProfileDto>> GetManagersAsync() => GetPartnerManagersByPartnerIdAsync(int.Parse(UserId!));
    public Task<PartnerManagerProfileDto> GetManagerByManagerIdAsync(int managerId) => GetPartnerManagerByPartnerIdAndManagerIdAsync(int.Parse(UserId!), managerId);
    public Task InviteManagerAsync(string email, CancellationToken cancellationToken = default) => InvitePartnerManagerAsync(int.Parse(UserId!), email, cancellationToken);
    public Task ResendManagerInvitationAsync(int managerId, CancellationToken cancellationToken = default) => ResendPartnerManagerInvitationAsync(int.Parse(UserId!), managerId, cancellationToken);
    public Task DeleteManagerAsync(int managerId, CancellationToken cancellationToken = default) => DeletePartnerManagerAsync(int.Parse(UserId!), managerId, cancellationToken);
    public Task ActivateManagerAsync(int managerId, CancellationToken cancellationToken = default) => ActivatePartnerManagerAsync(int.Parse(UserId!), managerId, cancellationToken);
    public Task DeactivateManagerAsync(int managerId, CancellationToken cancellationToken = default) => DeactivatePartnerManagerAsync(int.Parse(UserId!), managerId, cancellationToken);
    public Task<List<PartnerManagerProfileDto>> GetInvitedManagersAsync() => GetInvitedPartnerManagersByPartnerIdAsync(int.Parse(UserId!));
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
    #endregion
}