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

public class UserService(
    ICurrentUser currentUser,
    UserManager<User> userManager,
    IImageService imageService,
    IEmailSender emailSender,
    IAppSettings appSettings,
    IApplicationDbContext dbContext) : IUserService
{
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
            var adminRoleId = await dbContext.Roles.AsNoTracking().Where(r => r.Name == Roles.Admin).Select(r => r.Id).FirstAsync(cancellationToken);
            var adminCount = await dbContext.UserRoles.Where(ur => ur.RoleId == adminRoleId).CountAsync(cancellationToken);
            if (adminCount <= 1)
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

    public async Task<List<T>> GetUsersByRoleAsync<T>(string roleName, bool? confirmedOnly = null) where T : BaseUserProfile, new()
    {
        var roleId = await dbContext.Roles.AsNoTracking().Where(r => r.Name == roleName).Select(r => r.Id).FirstOrDefaultAsync();

        var query = dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.RoleId == roleId)
            .Join(dbContext.Users, ur => ur.UserId, u => u.Id, (ur, u) => u);

        if (confirmedOnly.HasValue)
            query = query.Where(u => u.EmailConfirmed == confirmedOnly.Value);

        var users = await query.ToListAsync();
        return [.. users.Select(MapToDto<T>)];
    }

    public async Task<T?> GetUserWithRoleAsync<T>(int userId, string roleName) where T : BaseUserProfile, new()
    {
        var roleId = await dbContext.Roles.AsNoTracking().Where(r => r.Name == roleName).Select(r => r.Id).FirstOrDefaultAsync();

        var user = await dbContext.UserRoles
            .AsNoTracking()
            .Where(ur => ur.UserId == userId && ur.RoleId == roleId)
            .Join(dbContext.Users, ur => ur.UserId, u => u.Id, (ur, u) => u)
            .FirstOrDefaultAsync();

        return user == null ? null : MapToDto<T>(user);
    }

    public async Task<User> CreateInvitedUserAsync(string email, string role, CancellationToken cancellationToken = default)
    {
        if (await userManager.FindByEmailAsync(email) != null)
            throw new ConflictError("Email already exists.");

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

    public async Task SendInvitationEmailAsync(User user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var inviteUrl = $"{appSettings.FrontendUrl}/accept-invitation?email={user.Email}&token={encodedToken}";
        await emailSender.SendEmailAsync(user.Email!, "You're invited to Luga Store",
            $"You've been invited. Set your password here: {inviteUrl}");
    }

    public async Task<User> GetUserWithRoleAsync(int userId, string requiredRole)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundError("User not found.");
        if (!await userManager.IsInRoleAsync(user, requiredRole))
            throw new ForbiddenError($"User is not in {requiredRole} role.");
        return user;
    }

    private static T MapToDto<T>(User user) where T : BaseUserProfile
    {
        if (typeof(T) == typeof(AdminProfileDto)) return (T)(BaseUserProfile)AdminProfileDto.From(user);
        if (typeof(T) == typeof(PartnerProfileDto)) return (T)(BaseUserProfile)PartnerProfileDto.From(user);
        if (typeof(T) == typeof(PartnerManagerProfileDto)) return (T)(BaseUserProfile)PartnerManagerProfileDto.From(user);
        return (T)(BaseUserProfile)CustomerProfileDto.From(user);
    }

    public async Task<T> GetProfileAsync<T>() where T : BaseUserProfile
        => MapToDto<T>(await GetCurrentUserAsync());

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

    public async Task DeleteAccountAsync() => await DeleteUserAsync(int.Parse(currentUser.UserId!));

    public async Task<bool> DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        if (await userManager.IsInRoleAsync(user, Roles.Admin))
        {
            var adminRoleId = await dbContext.Roles.AsNoTracking().Where(r => r.Name == Roles.Admin).Select(r => r.Id).FirstAsync(cancellationToken);
            var adminCount = await dbContext.UserRoles.Where(ur => ur.RoleId == adminRoleId).CountAsync(cancellationToken);
            if (adminCount <= 1)
                throw new BadRequestError("Cannot delete the last admin.");
        }

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    private async Task<User> GetCurrentUserAsync()
        => await userManager.FindByIdAsync(currentUser.UserId!) ?? throw new NotFoundError("User not found.");

    public Task<List<AdminProfileDto>> GetInvitedAdminsAsync()
        => GetUsersByRoleAsync<AdminProfileDto>(Roles.Admin, confirmedOnly: false);

    public Task<List<AdminProfileDto>> GetAdminsAsync()
        => GetUsersByRoleAsync<AdminProfileDto>(Roles.Admin);

    public Task<AdminProfileDto?> GetAdminAsync(int userId)
        => GetUserWithRoleAsync<AdminProfileDto>(userId, Roles.Admin);

    public async Task InviteAdminAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await CreateInvitedUserAsync(email, Roles.Admin, cancellationToken);
        await SendInvitationEmailAsync(user);
    }

    public async Task ResendAdminInvitationAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await GetUserWithRoleAsync(userId, Roles.Admin);
        if (user.EmailConfirmed)
            throw new BadRequestError("User has already confirmed their email.");
        await SendInvitationEmailAsync(user);
    }

    public Task<List<CustomerProfileDto>> GetCustomersAsync()
        => GetUsersByRoleAsync<CustomerProfileDto>(Roles.Customer);

    public Task<CustomerProfileDto?> GetCustomerAsync(int userId)
        => GetUserWithRoleAsync<CustomerProfileDto>(userId, Roles.Customer);

    public async Task<List<AddressDto>> GetAddressesAsync(int userId, CancellationToken cancellationToken = default)
    {
        var addresses = await dbContext.Addresses
            .AsNoTracking()
            .Where(a => a.UserId == userId)
            .ToListAsync(cancellationToken);

        return [.. addresses.Select(a => new AddressDto
        {
            Id = a.Id,
            Label = a.Label,
            FullName = a.FullName,
            Email = a.Email,
            Phone = a.Phone,
            Street = a.Street,
            City = a.City,
            ZipCode = a.ZipCode
        })];
    }

    public async Task<AddressDto> AddAddressAsync(int userId, AddressDto dto, CancellationToken cancellationToken = default)
    {
        var count = await dbContext.Addresses.CountAsync(a => a.UserId == userId, cancellationToken);
        if (count >= 5)
            throw new BadRequestError("Users can have at most 5 addresses.");

        var address = new Address
        {
            UserId = userId,
            Label = dto.Label,
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            Street = dto.Street,
            City = dto.City,
            ZipCode = dto.ZipCode
        };

        dbContext.Addresses.Add(address);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new AddressDto
        {
            Id = address.Id,
            Label = address.Label,
            FullName = address.FullName,
            Email = address.Email,
            Phone = address.Phone,
            Street = address.Street,
            City = address.City,
            ZipCode = address.ZipCode
        };
    }

    public async Task DeleteAddressAsync(int userId, int addressId, CancellationToken cancellationToken = default)
    {
        var address = await dbContext.Addresses
            .FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId, cancellationToken)
            ?? throw new NotFoundError("Address not found.");

        dbContext.Addresses.Remove(address);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}


