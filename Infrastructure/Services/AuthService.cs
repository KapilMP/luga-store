using System.Security.Claims;
using System.Text;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Google.Apis.Auth;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
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
    IConfiguration configuration) : IAuthService
{
    private string FrontendUrl => configuration["FrontendUrl"] ?? "https://luga-store.com";
    public async Task<AuthResult> LoginAsync(string email, string password)
    {
        var user = await userManager.FindByEmailAsync(email) ?? throw new NotFoundError("Email or Password is not correct");
        if (!user.EmailConfirmed)
            throw new NotFoundError("Please verify your email before logging in.");

        if (!user.IsActive)
            throw new UnauthorizedException("Your account has been deactivated.");

        var result = await signInManager.CheckPasswordSignInAsync(user, password, false);
        if (!result.Succeeded)
            throw new NotFoundError("Email or Password is not correct");

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = GenerateAccessToken(user, roles);
        var refreshToken = GenerateRefreshToken(user);

        return new AuthResult { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public async Task<AuthResult?> LoginWithGoogleAsync(string idToken)
    {
        try
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { googleSettings.ClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
            if (payload == null) return null;

            return await LoginExternalAsync(payload.Email, payload.GivenName ?? "", payload.FamilyName ?? "");
        }
        catch (InvalidJwtException)
        {
            return null;
        }
    }

    public async Task<AuthResult?> LoginExternalAsync(string email, string firstName, string lastName)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                UserName = email,
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

        return new AuthResult { AccessToken = accessToken, RefreshToken = refreshToken };
    }

    public async Task<(string AccessToken, string RefreshToken)?> RefreshTokenAsync(string refreshTokenValue)
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

    public async Task<string?> GenerateRefreshTokenAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return null;

        return GenerateRefreshToken(user);
    }

    // Identity Management
    public async Task SendVerificationEmailAsync(string email)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return;

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);

        var verificationUrl = $"{FrontendUrl}/verify-email?userId={user.Id}&token={encodedToken}";
        await emailSender.SendEmailAsync(email, "Verify Your email", $"Please verify your account by clicking here: {verificationUrl}");
    }

    public async Task<bool> ConfirmEmailAsync(string userId, string token)
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

    public async Task<bool> ResetPasswordAsync(string email, string token, string newPassword)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var result = await userManager.ResetPasswordAsync(user, token, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> ChangePasswordAsync(string userId, string currentPassword, string newPassword)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return false;

        var result = await userManager.ChangePasswordAsync(user, currentPassword, newPassword);
        return result.Succeeded;
    }

    public async Task<bool> DeleteUserAsync(int userId)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded;
    }

    public async Task<bool> GuestCheckoutAsync(string email, string firstName, string lastName, string phone)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null) return true; // already exists, guest or signed up

        var user = new User
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
            HasSignedUp = false
        };

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded) return false;

        await userManager.AddToRoleAsync(user, Roles.Customer);
        return true;
    }

    public async Task<bool> RegisterAsync(string email, string password, string firstName, string lastName, string phone)
    {
        var existing = await userManager.FindByEmailAsync(email);
        if (existing != null)
        {
            if (existing.HasSignedUp) return false;

            existing.FirstName = firstName;
            existing.LastName = lastName;
            existing.PhoneNumber = phone;
            existing.HasSignedUp = true;
            await userManager.UpdateAsync(existing);
            var addPasswordResult = await userManager.AddPasswordAsync(existing, password);
            if (!addPasswordResult.Succeeded) return false;

            await SendVerificationEmailAsync(email);
            return true;
        }

        var user = new User
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            PhoneNumber = phone,
            HasSignedUp = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded) return false;

        await userManager.AddToRoleAsync(user, Roles.Customer);
        await SendVerificationEmailAsync(email);
        return true;
    }

    public Task<bool> CreateAdminAsync(string email, string firstName, string lastName)
        => CreateInvitedUserAsync(email, firstName, lastName, Roles.Admin);

    public Task<bool> CreatePartnerAsync(string email, string firstName, string lastName)
        => CreateInvitedUserAsync(email, firstName, lastName, Roles.Partner);

    public Task<bool> CreatePartnerManagerAsync(string email, string firstName, string lastName)
        => CreateInvitedUserAsync(email, firstName, lastName, Roles.PartnerManager);

    public async Task<bool> AcceptInvitationAsync(string email, string token, string password)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null || user.EmailConfirmed) return false;

        var result = await userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded) return false;

        var addPassword = await userManager.AddPasswordAsync(user, password);
        return addPassword.Succeeded;
    }

    public async Task<bool> SetUserActiveStatusAsync(int userId, bool isActive)
    {
        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null) return false;

        user.IsActive = isActive;
        var result = await userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    private async Task<bool> CreateInvitedUserAsync(string email, string firstName, string lastName, string role)
    {
        if (await userManager.FindByEmailAsync(email) != null)
            throw new ConflictException("Email already exists.");

        var user = new User
        {
            UserName = email,
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            HasSignedUp = true,
            EmailConfirmed = false
        };

        var result = await userManager.CreateAsync(user);
        if (!result.Succeeded) throw new BadRequestException("Failed to create user.");

        await userManager.AddToRoleAsync(user, role);
        await SendInvitationEmailAsync(user);
        return true;
    }

    private async Task SendInvitationEmailAsync(User user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var inviteUrl = $"{FrontendUrl}/accept-invitation?email={user.Email}&token={encodedToken}";
        await emailSender.SendEmailAsync(user.Email!, "You're invited to Luga Store",
            $"Hi {user.FirstName}, you've been invited. Set your password here: {inviteUrl}");
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
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        return CreateToken(claims, jwtSettings.AccessTokenExpiryMinutes);
    }

    private string GenerateRefreshToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? "")
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
            if (securityToken is not JwtSecurityToken jwtSecurityToken || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                return null;

            return principal;
        }
        catch
        {
            return null;
        }
    }
}
