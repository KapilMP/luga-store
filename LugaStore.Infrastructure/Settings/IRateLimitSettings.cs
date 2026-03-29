namespace LugaStore.Infrastructure.Settings;

public interface IRateLimitSettings
{
    int RejectionStatusCode { get; init; }
    Dictionary<string, RateLimitPolicySettings> Policies { get; init; }
}

public record RateLimitPolicySettings
{
    public string Window { get; init; } = "00:01:00";
    public int PermitLimit { get; init; } = 100;
    public int QueueLimit { get; init; } = 0;
    public string QueueProcessingOrder { get; init; } = "OldestFirst";
}
