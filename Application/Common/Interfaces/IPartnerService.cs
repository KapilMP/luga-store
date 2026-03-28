using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Common.Interfaces;

public interface IPartnerService
{
    // For Admin
    Task InvitePartnerAsync(string email, CancellationToken cancellationToken = default);
    Task InvitePartnerManagerAsync(int partnerId, string email, CancellationToken cancellationToken = default);
    Task ResendPartnerManagerInvitationAsync(int partnerId, int managerId, CancellationToken cancellationToken = default);
    Task DeletePartnerManagerAsync(int partnerId, int managerId, CancellationToken cancellationToken = default);
    Task ActivatePartnerAsync(int partnerId, CancellationToken cancellationToken = default);
    Task DeactivatePartnerAsync(int partnerId, CancellationToken cancellationToken = default);
    Task ActivatePartnerManagerAsync(int partnerId, int managerId, CancellationToken cancellationToken = default);
    Task DeactivatePartnerManagerAsync(int partnerId, int managerId, CancellationToken cancellationToken = default);
    Task DeletePartnerAsync(int partnerId, CancellationToken cancellationToken = default);
    Task ResendPartnerInvitationAsync(int partnerId, CancellationToken cancellationToken = default);

    Task<PartnerProfileDto> GetPartnerByPartnerIdAsync(int partnerId);
    Task<List<PartnerProfileDto>> GetPartnersAsync();
    Task<PartnerManagerProfileDto> GetPartnerManagerByPartnerIdAndManagerIdAsync(int partnerId, int managerId);
    Task<List<PartnerManagerProfileDto>> GetPartnerManagersByPartnerIdAsync(int partnerId);
    Task<List<PartnerProfileDto>> GetInvitedPartnersAsync();
    Task<List<PartnerManagerProfileDto>> GetInvitedPartnerManagersByPartnerIdAsync(int partnerId);

    // For Partner
    Task<List<PartnerManagerProfileDto>> GetManagersAsync();
    Task<PartnerManagerProfileDto> GetManagerByManagerIdAsync(int managerId);
    Task InviteManagerAsync(string email, CancellationToken cancellationToken = default);
    Task ResendManagerInvitationAsync(int managerId, CancellationToken cancellationToken = default);
    Task DeleteManagerAsync(int managerId, CancellationToken cancellationToken = default);
    Task ActivateManagerAsync(int managerId, CancellationToken cancellationToken = default);
    Task DeactivateManagerAsync(int managerId, CancellationToken cancellationToken = default);
    Task<List<PartnerManagerProfileDto>> GetInvitedManagersAsync();
}