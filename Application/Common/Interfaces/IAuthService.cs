using System.Threading.Tasks;

using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Common.Interfaces;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(string email, string password);
    Task<AuthResult?> LoginWithGoogleAsync(string idToken);
    Task<AuthResult?> LoginExternalAsync(string email, string firstName, string lastName);
    Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshToken);
    Task<string?> GenerateRefreshTokenAsync(string email);

    // Identity Management
    Task SendVerificationEmailAsync(string email);
    Task<bool> ConfirmEmailAsync(string userId, string token);
    Task ForgotPasswordAsync(string email);
    Task<bool> ResetPasswordAsync(string email, string token, string newPassword);
    Task<bool> DeleteUserAsync(int userId);

    // User creation rules
    Task<bool> GuestCheckoutAsync(string email, string firstName, string lastName, string phone);
    Task<bool> RegisterAsync(string email, string password, string firstName, string lastName, string phone);
    Task<bool> CreateAdminAsync(string email, string firstName, string lastName);
    Task<bool> CreatePartnerAsync(string email, string firstName, string lastName);
    Task<bool> CreatePartnerManagerAsync(string email, string firstName, string lastName);
    Task<bool> AcceptInvitationAsync(string email, string token, string password);
    Task<bool> SetUserActiveStatusAsync(int userId, bool isActive);
}
