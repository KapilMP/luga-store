using System.Security.Claims;
using System.Text;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Infrastructure.Persistence;
using LugaStore.Infrastructure.Settings;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.Infrastructure.Services;

public class AuthService(
    UserManager<User> userManager,
    SignInManager<User> signInManager,
    IJwtSettings jwtSettings,
    IGoogleSettings googleSettings,
    IEmailSender emailSender,
    IAppSettings appSettings) : IAuthService
{
    private string FrontendUrl => appSettings.FrontendUrl;

    public async Task<AuthResult> CustomerLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var (accessToken, refreshToken, user) = await LoginWithRoleAsync(email, password, Roles.Customer, cancellationToken);
        if (user.PasswordHash == null) throw new NotFoundError("Email or Password is not correct");
        return new AuthResult { AccessToken = accessToken, RefreshToken = refreshToken, User = CustomerProfileDto.From(user) };
    }

    public async Task<AuthResult> AdminLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var (accessToken, refreshToken, user) = await LoginWithRoleAsync(email, password, Roles.Admin, cancellationToken);
        if (!user.EmailConfirmed) throw new NotFoundError("Email or Password is not correct");
        return new AuthResult { AccessToken = accessToken, RefreshToken = refreshToken, User = AdminProfileDto.From(user) };
    }

    public async Task<AuthResult> PartnerLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var (accessToken, refreshToken, user) = await LoginWithRoleAsync(email, password, Roles.Partner, cancellationToken);
        if (!user.EmailConfirmed) throw new NotFoundError("Email or Password is not correct");
        return new AuthResult { AccessToken = accessToken, RefreshToken = refreshToken, User = PartnerProfileDto.From(user) };
    }

    public async Task<AuthResult> PartnerManagerLoginAsync(string email, string password, CancellationToken cancellationToken = default)
    {
        var (accessToken, refreshToken, user) = await LoginWithRoleAsync(email, password, Roles.PartnerManager, cancellationToken);
        if (!user.EmailConfirmed) throw new NotFoundError("Email or Password is not correct");
        return new AuthResult { AccessToken = accessToken, RefreshToken = refreshToken, User = PartnerManagerProfileDto.From(user) };
    }

    private async Task<(string AccessToken, string RefreshToken, User User)> LoginWithRoleAsync(string email, string password, string requiredRole, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email) ?? throw new NotFoundError("Email or Password is not correct");

        if (!user.IsActive)
            throw new UnauthorizedError("Your account has been deactivated.");

        var signInResult = await signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!signInResult.Succeeded)
            throw new NotFoundError("Email or Password is not correct");

        if (!await userManager.IsInRoleAsync(user, requiredRole))
            throw new NotFoundError("Email or Password is not correct");

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = GenerateAccessToken(user, roles);
        var refreshToken = GenerateRefreshToken(user);

        return (accessToken, refreshToken, user);
    }

    public async Task<AuthResult?> LoginWithGoogleAsync(string idToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = [googleSettings.ClientId]
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            if (payload == null) return null;

            return await LoginExternalAsync(payload.Email, payload.GivenName ?? "", payload.FamilyName ?? "", cancellationToken);
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }

    public async Task<AuthResult?> LoginExternalAsync(string email, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded) return null;

            await userManager.AddToRoleAsync(user, Roles.Customer);
        }

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = GenerateAccessToken(user, roles);
        var refreshToken = GenerateRefreshToken(user);

        return new AuthResult
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            User = CustomerProfileDto.From(user)
        };
    }

    public async Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshTokenValue, CancellationToken cancellationToken = default)
    {
        var principal = GetPrincipalFromExpiredToken(refreshTokenValue);
        if (principal == null) return null;

        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return null;

        var user = await userManager.FindByEmailAsync(email);
        if (user == null || !user.IsActive) return null;

        var roles = await userManager.GetRolesAsync(user);
        var newAccessToken = GenerateAccessToken(user, roles);
        var newRefreshToken = GenerateRefreshToken(user);

        return (newAccessToken, newRefreshToken);
    }

    public async Task<string?> GenerateRefreshTokenAsync(string email, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return null;

        return GenerateRefreshToken(user);
    }

    public async Task SendVerificationEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return;

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var verificationUrl = $"{FrontendUrl}/verify-email?userId={user.Id}&token={encodedToken}";
        await emailSender.SendEmailAsync(email, "Verify Your email", $"Please verify your account by clicking here: {verificationUrl}");
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task ForgotPasswordAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var resetUrl = $"{FrontendUrl}/reset-password?email={email}&token={encodedToken}";
        await emailSender.SendEmailAsync(email, "Reset Password", $"Reset your password by clicking here: {resetUrl}");
    }

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }



    public async Task<bool> GuestCheckoutAsync(string email, string firstName, string lastName, string phone, CancellationToken cancellationToken = default)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null) return true;

        var user = new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
        };

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded) return false;

        await userManager.AddToRoleAsync(user, Roles.Customer);
        return true;
    }

    public async Task<bool> RegisterAsync(string email, string password, string firstName, string lastName, string phone, CancellationToken cancellationToken = default)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            if (existing.PasswordHash != null) return false;

            existing.FirstName = firstName;
            existing.LastName = lastName;
            existing.PhoneNumber = phone;
            await userManager.UpdateAsync(existing);
            var addPasswordResult = await userManager.AddPasswordAsync(existing, password);
            if (!addPasswordResult.Succeeded) return false;

            await SendVerificationEmailAsync(email);
            return true;
        }

        var user = new User
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded) return false;

        await userManager.AddToRoleAsync(user, Roles.Customer);
        await SendVerificationEmailAsync(email);
        return true;
    }

    public async Task<bool> AcceptInvitationAsync(string email, string token, string password, string firstName, string lastName, CancellationToken cancellationToken = default)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null || user.EmailConfirmed) return false;

        user.FirstName = firstName;
        user.LastName = lastName;
        await userManager.UpdateAsync(user);

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded) return false;

        var addPassword = await userManager.AddPasswordAsync(user, password);
        return addPassword.Succeeded;
    }


    private string GenerateAccessToken(User user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? ""),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        foreach (var role in roles)
            claims.Add(new Claim(ClaimTypes.Role, role));

        return CreateToken(claims, jwtSettings.AccessTokenExpiryMinutes);
    }

    private string GenerateRefreshToken(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? "")
        };

        return CreateToken(claims, jwtSettings.RefreshTokenExpiryDays * 24 * 60);
    }

    private string CreateToken(IEnumerable<Claim> claims, int expiryMinutes)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwtSettings.Issuer,
            audience: jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
    {
        var tokenValidationParameters = new TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret)),
            ValidateLifetime = true
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out SecurityToken securityToken);
            if (securityToken is not JwtSecurityToken jwtSecurityToken ||
                !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
