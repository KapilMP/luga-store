namespace LugaStore.Application.Common.Interfaces;

public record GoogleUser(string Email, string GivenName, string FamilyName);

public interface IGoogleAuthService
{
    Task<GoogleUser?> ValidateTokenAsync(string idToken);
}
