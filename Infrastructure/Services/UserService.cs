using System.Security.Claims;
using System.Web;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;
using LugaStore.Infrastructure.Persistence;
using LugaStore.Infrastructure.Settings;

namespace LugaStore.Infrastructure.Services;

public class UserService(
    IHttpContextAccessor httpContextAccessor,
    UserManager<User> userManager,
    IImageService imageService,
    IEmailSender emailSender,
    IAppSettings appSettings,
    ApplicationDbContext dbContext) : IUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Role => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

    public async Task<bool> SetUserActiveStatusAsync(int userId, bool isActive, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.IsActive = isActive;
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    private async Task SetActiveStatusWithRoleCheckAsync(int userId, bool isActive, string role, CancellationToken cancellationToken)
    {
        var user = await GetUserWithRoleAsync(userId, role);

        user.IsActive = isActive;
        await userManager.UpdateAsync(user);
    }

    private async Task DeleteWithRoleCheckAsync(int userId, string role, CancellationToken cancellationToken)
    {
        var user = await GetUserWithRoleAsync(userId, role);

        if (role == Roles.Admin)
        {
            var admins = await userManager.GetUsersInRoleAsync(Roles.Admin);
            if (admins.Count <= 1)
                throw new BadRequestError("Cannot delete the last admin.");
        }

        await userManager.DeleteAsync(user);
    }

    public Task ActivateAdminAsync(int userId, CancellationToken cancellationToken = default)
        => SetActiveStatusWithRoleCheckAsync(userId, true, Roles.Admin, cancellationToken);

    public Task DeactivateAdminAsync(int userId, CancellationToken cancellationToken = default)
        => SetActiveStatusWithRoleCheckAsync(userId, false, Roles.Admin, cancellationToken);

    public Task DeleteAdminAsync(int userId, CancellationToken cancellationToken = default)
        => DeleteWithRoleCheckAsync(userId, Roles.Admin, cancellationToken);
    public async Task<CustomerProfileDto?> GetCustomerAsync(int id)
    {
        var roleId = await dbContext.Roles.AsNoTracking().Where(r => r.Name == Roles.Customer).Select(r => r.Id).FirstOrDefaultAsync();
        
        return await dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == id && ur.RoleId == roleId)
            .Join(dbContext.Users, ur => ur.UserId, u => u.Id, (ur, u) => new CustomerProfileDto
            {
                Id = u.Id,
                FirstName = u.FirstName ?? string.Empty,
                LastName = u.LastName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                AvatarUrl = u.AvatarPath ?? string.Empty,
                Phone = u.PhoneNumber ?? string.Empty,
                IsEmailConfirmed = u.EmailConfirmed
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<CustomerProfileDto>> GetCustomersAsync()
    {
        var roleId = await dbContext.Roles.AsNoTracking().Where(r => r.Name == Roles.Customer).Select(r => r.Id).FirstOrDefaultAsync();

        return await dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.RoleId == roleId)
            .Join(dbContext.Users, ur => ur.UserId, u => u.Id, (ur, u) => new CustomerProfileDto
            {
                Id = u.Id,
                FirstName = u.FirstName ?? string.Empty,
                LastName = u.LastName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                AvatarUrl = u.AvatarPath ?? string.Empty,
                Phone = u.PhoneNumber ?? string.Empty,
                IsEmailConfirmed = u.EmailConfirmed
            })
            .ToListAsync();
    }

    public async Task<AdminProfileDto?> GetAdminAsync(int id)
    {
        var roleId = await dbContext.Roles.AsNoTracking().Where(r => r.Name == Roles.Admin).Select(r => r.Id).FirstOrDefaultAsync();

        return await dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == id && ur.RoleId == roleId)
            .Join(dbContext.Users, ur => ur.UserId, u => u.Id, (ur, u) => new AdminProfileDto
            {
                Id = u.Id,
                FirstName = u.FirstName ?? string.Empty,
                LastName = u.LastName ?? string.Empty,
                Email = u.Email ?? string.Empty,
                AvatarUrl = u.AvatarPath ?? string.Empty,
                Phone = u.PhoneNumber ?? string.Empty
            })
            .FirstOrDefaultAsync();
    }

    public async Task<List<AdminProfileDto>> GetAdminsAsync()
    {
        var roleId = await dbContext.Roles.AsNoTracking().Where(r => r.Name == Roles.Admin).Select(r => r.Id).FirstOrDefaultAsync();

        return await dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.RoleId == roleId)
            .Join(dbContext.Users, ur => ur.UserId, u => u.Id, (ur, u) => new AdminProfileDto
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

    public async Task InviteAdminAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await CreateInvitedUserAsync(email, Roles.Admin, cancellationToken);
        await SendInvitationEmailAsync(user);
    }

    public async Task ResendAdminInvitationAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await GetUserWithRoleAsync(userId, Roles.Admin);
        if (user.EmailConfirmed) throw new BadRequestError("User has already accepted the invitation.");
        await SendInvitationEmailAsync(user);
    }

    private async Task<User> CreateInvitedUserAsync(string email, string role, CancellationToken cancellationToken)
    {
        if (await userManager.FindByEmailAsync(email) != null)
            throw new ConflictError("Email already exists.");

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = false
            };

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
        await emailSender.SendEmailAsync(user.Email!, "You're invited to Luga Store",
            $"You've been invited. Set your password here: {inviteUrl}");
    }

    private async Task<User> GetCurrentUserAsync()
        => await userManager.FindByIdAsync(UserId!) ?? throw new NotFoundError("User not found.");

    private static T MapToDto<T>(User user) where T : BaseUserProfile
    {
        if (typeof(T) == typeof(AdminProfileDto)) return (T)(BaseUserProfile)AdminProfileDto.From(user);
        if (typeof(T) == typeof(PartnerProfileDto)) return (T)(BaseUserProfile)PartnerProfileDto.From(user);
        if (typeof(T) == typeof(PartnerManagerProfileDto)) return (T)(BaseUserProfile)PartnerManagerProfileDto.From(user);
        return (T)(BaseUserProfile)CustomerProfileDto.From(user);
    }

    public async Task<T> GetProfileAsync<T>() where T : BaseUserProfile
    {
        var user = await GetCurrentUserAsync();
        return MapToDto<T>(user);
    }

    public async Task<T> UpdateProfileAsync<T>(string firstName, string lastName, string phone) where T : BaseUserProfile
    {
        var user = await GetCurrentUserAsync();
        user.FirstName = firstName;
        user.LastName = lastName;
        user.PhoneNumber = phone;
        await userManager.UpdateAsync(user);
        return MapToDto<T>(user);
    }

    public async Task<T> UploadAvatarAsync<T>(Stream stream, string fileName, CancellationToken cancellationToken = default) where T : BaseUserProfile
    {
        var user = await GetCurrentUserAsync();
        user.AvatarPath = await imageService.UploadAvatarAsync(stream, fileName, cancellationToken);
        await userManager.UpdateAsync(user);
        return MapToDto<T>(user);
    }

    public async Task DeleteAccountAsync()
    {
        var user = await GetCurrentUserAsync();
        await DeleteUserAsync(user.Id);
    }

    public async Task<bool> DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        if (await userManager.IsInRoleAsync(user, Roles.Admin))
        {
            var roleId = await dbContext.Roles.AsNoTracking().Where(r => r.Name == Roles.Admin).Select(r => r.Id).FirstOrDefaultAsync();
            var adminCount = await dbContext.UserRoles.Where(ur => ur.RoleId == roleId).CountAsync(cancellationToken);
            if (adminCount <= 1)
                throw new BadRequestError("Cannot delete the last admin.");
        }

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }
    public async Task<List<AdminProfileDto>> GetInvitedAdminsAsync()
    {
        var roleId = await dbContext.Roles.AsNoTracking().Where(r => r.Name == Roles.Admin).Select(r => r.Id).FirstOrDefaultAsync();

        return await dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.RoleId == roleId)
            .Join(dbContext.Users.Where(u => !u.EmailConfirmed), ur => ur.UserId, u => u.Id, (ur, u) => new AdminProfileDto
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

    public async Task<User> GetUserWithRoleAsync(int userId, string requiredRole)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundError("User not found.");
        if (!await userManager.IsInRoleAsync(user, requiredRole))
            throw new ForbiddenError($"User is not in {requiredRole} role.");
        return user;
    }
}
