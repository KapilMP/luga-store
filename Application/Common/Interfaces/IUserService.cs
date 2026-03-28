using Microsoft.AspNetCore.Http;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Common.Interfaces;

public interface IUserService
{
    string? UserId { get; }
    string? Role { get; }
    Task DeleteAdminAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> SetUserActiveStatusAsync(int userId, bool isActive, CancellationToken cancellationToken = default);
    Task ActivateAdminAsync(int userId, CancellationToken cancellationToken = default);
    Task DeactivateAdminAsync(int userId, CancellationToken cancellationToken = default);

    Task<CustomerProfileDto?> GetCustomerAsync(int id);
    Task<List<CustomerProfileDto>> GetCustomersAsync();
    Task<AdminProfileDto?> GetAdminAsync(int id);
    Task<List<AdminProfileDto>> GetAdminsAsync();
    Task InviteAdminAsync(string email, CancellationToken cancellationToken = default);
    Task ResendAdminInvitationAsync(int userId, CancellationToken cancellationToken = default);
    Task<T> GetProfileAsync<T>() where T : BaseUserProfile;
    Task<T> UpdateProfileAsync<T>(string firstName, string lastName, string phone) where T : BaseUserProfile;
    Task<T> UploadAvatarAsync<T>(Stream stream, string fileName, CancellationToken cancellationToken = default) where T : BaseUserProfile;
    Task DeleteAccountAsync();
    Task<bool> DeleteUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<List<AdminProfileDto>> GetInvitedAdminsAsync();
    Task<User> GetUserWithRoleAsync(int userId, string requiredRole);
}
