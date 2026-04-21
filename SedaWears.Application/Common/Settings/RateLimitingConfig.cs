namespace SedaWears.Application.Common.Settings;

public record RateLimitingConfig
{
    public int RejectionStatusCode { get; init; } = 429;
    public Dictionary<string, RateLimitPolicyConfig> Policies { get; init; } = new();
}
