using LugaStore.Application.Common.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Common.Interfaces;

public interface IUserService
{
    Task<List<T>> GetUsersByRoleAsync<T>(string roleName, bool? confirmedOnly = null) where T : BaseUserProfile, new();
    Task<T?> GetUserWithRoleAsync<T>(int userId, string roleName) where T : BaseUserProfile, new();

    Task<User> CreateInvitedUserAsync(string email, string role, CancellationToken cancellationToken = default);
    Task SendInvitationEmailAsync(User user);

    Task DeleteAdminAsync(int userId, CancellationToken cancellationToken = default);
    Task<bool> SetUserActiveStatusAsync(int userId, bool isActive, CancellationToken cancellationToken = default);
    Task ActivateAdminAsync(int userId, CancellationToken cancellationToken = default);
    Task DeactivateAdminAsync(int userId, CancellationToken cancellationToken = default);

    Task<T> GetProfileAsync<T>() where T : BaseUserProfile;
    Task<T> UpdateProfileAsync<T>(string firstName, string lastName, string phone) where T : BaseUserProfile;
    Task<T> UploadAvatarAsync<T>(Stream stream, string fileName, CancellationToken cancellationToken = default) where T : BaseUserProfile;
    Task DeleteAccountAsync();
    Task<bool> DeleteUserAsync(int userId, CancellationToken cancellationToken = default);
    Task<User> GetUserWithRoleAsync(int userId, string requiredRole);
    
    Task<List<AdminProfileDto>> GetInvitedAdminsAsync();
    Task<List<AdminProfileDto>> GetAdminsAsync();
    Task<AdminProfileDto?> GetAdminAsync(int userId);
    Task InviteAdminAsync(string email, CancellationToken cancellationToken = default);
    Task ResendAdminInvitationAsync(int userId, CancellationToken cancellationToken = default);
    
    Task<List<CustomerProfileDto>> GetCustomersAsync();
    Task<CustomerProfileDto?> GetCustomerAsync(int userId);
    
    // Address Management
    Task<AddressDto> AddAddressAsync(int userId, AddressDto addressDto, CancellationToken cancellationToken = default);
    Task DeleteAddressAsync(int userId, int addressId, CancellationToken cancellationToken = default);
    Task<List<AddressDto>> GetAddressesAsync(int userId, CancellationToken cancellationToken = default);
}


