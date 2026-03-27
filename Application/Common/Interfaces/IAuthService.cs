using System.Threading.Tasks;

using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResult> CustomerLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResult> AdminLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResult> PartnerLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResult> PartnerManagerLoginAsync(string email, string password, CancellationToken cancellationToken = default);
    Task<AuthResult?> LoginWithGoogleAsync(string idToken, CancellationToken cancellationToken = default);
    Task<AuthResult?> LoginExternalAsync(string email, string firstName, string lastName, CancellationToken cancellationToken = default);
    Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);
    Task<string?> GenerateRefreshTokenAsync(string email, CancellationToken cancellationToken = default);

    // Identity Management
    Task SendVerificationEmailAsync(string email);
    Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default);
    Task ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default);
    Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default);
    Task<bool> DeleteUserAsync(int userId, CancellationToken cancellationToken = default);

    // User creation rules
    Task<bool> GuestCheckoutAsync(string email, string firstName, string lastName, string phone, CancellationToken cancellationToken = default);
    Task<bool> RegisterAsync(string email, string password, string firstName, string lastName, string phone, CancellationToken cancellationToken = default);
    Task<bool> InviteAdminAsync(string email, string firstName, string lastName, CancellationToken cancellationToken = default);
    Task<bool> InvitePartnerAsync(string email, string firstName, string lastName, CancellationToken cancellationToken = default);
    Task<bool> InvitePartnerManagerAsync(string email, string firstName, string lastName, int partnerId, CancellationToken cancellationToken = default);
    Task<bool> AcceptInvitationAsync(string email, string token, string password, CancellationToken cancellationToken = default);
    Task<bool> ResendInvitationAsync(string email);
    Task<bool> SetUserActiveStatusAsync(int userId, bool isActive, CancellationToken cancellationToken = default);
}
