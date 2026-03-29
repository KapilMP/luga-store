namespace LugaStore.Infrastructure.Settings;

public record RateLimitSettings : IRateLimitSettings
{
    public int RejectionStatusCode { get; init; } = 429;
    public Dictionary<string, RateLimitPolicySettings> Policies { get; init; } = new();
}
