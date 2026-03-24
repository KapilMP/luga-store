namespace LugaStore.Application.Common.Interfaces;

public interface IJwtSettings
{
    string Secret { get; init; }
    string Issuer { get; init; }
    string Audience { get; init; }
    int AccessTokenExpiryMinutes { get; init; }
    int RefreshTokenExpiryDays { get; init; }
}
