using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Common.Models;

public record JwtSettings : IJwtSettings
{
    public string Secret { get; init; } = string.Empty;
    public string Issuer { get; init; } = string.Empty;
    public string Audience { get; init; } = string.Empty;
    public int AccessTokenExpiryMinutes { get; init; }
    public int RefreshTokenExpiryDays { get; init; }
}
