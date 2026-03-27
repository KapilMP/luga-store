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
    IAuthService authService,
    IImageService imageService,
    IEmailSender emailSender,
    IAppSettings appSettings,
    ApplicationDbContext dbContext) : IUserService
{
    public string? UserId => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    public string? Role => httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.Role);

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }

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
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User not found.");
        if (!await userManager.IsInRoleAsync(user, role))
            throw new NotFoundException("User not found.");

        user.IsActive = isActive;
        await userManager.UpdateAsync(user);
    }

    private async Task DeleteWithRoleCheckAsync(int userId, string role, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User not found.");
        if (!await userManager.IsInRoleAsync(user, role))
            throw new NotFoundException("User not found.");

        if (role == Roles.Admin)
        {
            var admins = await userManager.GetUsersInRoleAsync(Roles.Admin);
            if (admins.Count <= 1)
                throw new BadRequestException("Cannot delete the last admin.");
        }

        await userManager.DeleteAsync(user);
    }

    public Task ActivateAdminAsync(int userId, CancellationToken cancellationToken = default)
        => SetActiveStatusWithRoleCheckAsync(userId, true, Roles.Admin, cancellationToken);

    public Task DeactivateAdminAsync(int userId, CancellationToken cancellationToken = default)
        => SetActiveStatusWithRoleCheckAsync(userId, false, Roles.Admin, cancellationToken);

    public Task ActivatePartnerAsync(int userId, CancellationToken cancellationToken = default)
        => SetActiveStatusWithRoleCheckAsync(userId, true, Roles.Partner, cancellationToken);

    public Task DeactivatePartnerAsync(int userId, CancellationToken cancellationToken = default)
        => SetActiveStatusWithRoleCheckAsync(userId, false, Roles.Partner, cancellationToken);

    public Task ActivatePartnerManagerAsync(int userId, CancellationToken cancellationToken = default)
        => SetActiveStatusWithRoleCheckAsync(userId, true, Roles.PartnerManager, cancellationToken);

    public Task DeactivatePartnerManagerAsync(int userId, CancellationToken cancellationToken = default)
        => SetActiveStatusWithRoleCheckAsync(userId, false, Roles.PartnerManager, cancellationToken);

    public Task DeleteAdminAsync(int userId, CancellationToken cancellationToken = default)
        => DeleteWithRoleCheckAsync(userId, Roles.Admin, cancellationToken);

    public Task DeletePartnerAsync(int userId, CancellationToken cancellationToken = default)
        => DeleteWithRoleCheckAsync(userId, Roles.Partner, cancellationToken);

    public Task DeletePartnerManagerAsync(int userId, CancellationToken cancellationToken = default)
        => DeleteWithRoleCheckAsync(userId, Roles.PartnerManager, cancellationToken);
    public async Task<PartnerProfileDto?> GetPartnerAsync(int id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null || !await userManager.IsInRoleAsync(user, Roles.Partner)) return null;
        return PartnerProfileDto.From(user);
    }

    public async Task<List<PartnerProfileDto>> GetPartnersAsync()
    {
        var partners = await userManager.GetUsersInRoleAsync(Roles.Partner);
        return partners.Select(PartnerProfileDto.From).ToList();
    }

    public async Task<PartnerManagerProfileDto?> GetPartnerManagerAsync(int id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null || !await userManager.IsInRoleAsync(user, Roles.PartnerManager)) return null;
        return PartnerManagerProfileDto.From(user);
    }

    public async Task<List<PartnerManagerProfileDto>> GetPartnerManagersAsync()
    {
        var managers = await userManager.GetUsersInRoleAsync(Roles.PartnerManager);
        return managers.Select(PartnerManagerProfileDto.From).ToList();
    }

    public async Task<CustomerProfileDto?> GetCustomerAsync(int id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null || !await userManager.IsInRoleAsync(user, Roles.Customer)) return null;
        return CustomerProfileDto.From(user);
    }

    public async Task<List<CustomerProfileDto>> GetCustomersAsync()
    {
        var customers = await userManager.GetUsersInRoleAsync(Roles.Customer);
        return customers.Select(CustomerProfileDto.From).ToList();
    }

    public async Task<AdminProfileDto?> GetAdminAsync(int id)
    {
        var user = await userManager.FindByIdAsync(id.ToString());
        if (user == null || !await userManager.IsInRoleAsync(user, Roles.Admin)) return null;
        return AdminProfileDto.From(user);
    }

    public async Task<List<AdminProfileDto>> GetAdminsAsync()
    {
        var admins = await userManager.GetUsersInRoleAsync(Roles.Admin);
        return admins.Select(AdminProfileDto.From).ToList();
    }

    public async Task<bool> InviteAdminAsync(string email, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var user = await CreateInvitedUserAsync(email, firstName, lastName, Roles.Admin, cancellationToken);
        await SendInvitationEmailAsync(user);
        return true;
    }

    public async Task<bool> InvitePartnerAsync(string email, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var user = await CreateInvitedUserAsync(email, firstName, lastName, Roles.Partner, cancellationToken);
        await SendInvitationEmailAsync(user);
        return true;
    }

    public async Task<bool> InvitePartnerManagerAsync(string email, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var partnerId = int.Parse(UserId!);
        var user = await CreateInvitedUserAsync(email, firstName, lastName, Roles.PartnerManager, cancellationToken, partnerId);
        await SendInvitationEmailAsync(user);
        return true;
    }

    public async Task ResendInvitationAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email) ?? throw new NotFoundException("User not found.");
        if (user.EmailConfirmed) throw new BadRequestException("User has already accepted the invitation.");
        await SendInvitationEmailAsync(user);
    }

    private async Task<User> CreateInvitedUserAsync(string email, string firstName, string lastName, string role, CancellationToken cancellationToken, int? partnerId = null)
    {
        if (await userManager.FindByEmailAsync(email) != null)
            throw new ConflictException("Email already exists.");

        await using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        try
        {
            var user = new User
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = false,
                PartnerId = partnerId
            };

            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded) throw new InternalServerException("Failed to create user.");

            var roleResult = await userManager.AddToRoleAsync(user, role);
            if (!roleResult.Succeeded) throw new InternalServerException("Failed to assign role.");

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
            $"Hi {user.FirstName}, you've been invited. Set your password here: {inviteUrl}");
    }

    private async Task<User> GetCurrentUserAsync()
        => await userManager.FindByIdAsync(UserId!) ?? throw new NotFoundException("User not found.");

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
        await authService.DeleteUserAsync(user.Id);
    }
}
