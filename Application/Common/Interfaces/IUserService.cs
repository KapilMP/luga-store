using Microsoft.AspNetCore.Http;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Common.Interfaces;

public interface IUserService
{
    string? UserId { get; }
    string? Role { get; }
    Task<bool> DeleteUserAsync(int userId);
    Task DeleteAdminAsync(int userId, CancellationToken cancellationToken = default);
    Task DeletePartnerAsync(int userId, CancellationToken cancellationToken = default);
    Task DeletePartnerManagerAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> SetUserActiveStatusAsync(int userId, bool isActive, CancellationToken cancellationToken = default);
    Task ActivateAdminAsync(int userId, CancellationToken cancellationToken = default);
    Task DeactivateAdminAsync(int userId, CancellationToken cancellationToken = default);
    Task ActivatePartnerAsync(int userId, CancellationToken cancellationToken = default);
    Task DeactivatePartnerAsync(int userId, CancellationToken cancellationToken = default);
    Task ActivatePartnerManagerAsync(int userId, CancellationToken cancellationToken = default);
    Task DeactivatePartnerManagerAsync(int userId, CancellationToken cancellationToken = default);
    Task<PartnerProfileDto?> GetPartnerAsync(int id);
    Task<List<PartnerProfileDto>> GetPartnersAsync();
    Task<PartnerManagerProfileDto?> GetPartnerManagerAsync(int id);
    Task<List<PartnerManagerProfileDto>> GetPartnerManagersAsync();
    Task<CustomerProfileDto?> GetCustomerAsync(int id);
    Task<List<CustomerProfileDto>> GetCustomersAsync();
    Task<AdminProfileDto?> GetAdminAsync(int id);
    Task<List<AdminProfileDto>> GetAdminsAsync();
    Task<bool> InviteAdminAsync(string email, string firstName, string lastName, CancellationToken cancellationToken = default);
    Task<bool> InvitePartnerAsync(string email, string firstName, string lastName, CancellationToken cancellationToken = default);
    Task<bool> InvitePartnerManagerAsync(string email, string firstName, string lastName, CancellationToken cancellationToken = default);
    Task ResendInvitationAsync(string email);
    Task<T> GetProfileAsync<T>() where T : BaseUserProfile;
    Task<T> UpdateProfileAsync<T>(string firstName, string lastName, string phone) where T : BaseUserProfile;
    Task<T> UploadAvatarAsync<T>(Stream stream, string fileName, CancellationToken cancellationToken = default) where T : BaseUserProfile;
    Task DeleteAccountAsync();
}
