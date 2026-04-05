namespace LugaStore.Application.Common.Configurations;

public record RateLimitConfig
{
    public int RejectionStatusCode { get; init; } = 429;
    public Dictionary<string, RateLimitPolicyConfig> Policies { get; init; } = new();
}

public record RateLimitPolicyConfig
{
    public string Window { get; init; } = "00:01:00";
    public int PermitLimit { get; init; } = 100;
    public int QueueLimit { get; init; } = 0;
    public string QueueProcessingOrder { get; init; } = "OldestFirst";
}
