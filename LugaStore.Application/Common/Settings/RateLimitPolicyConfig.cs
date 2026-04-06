namespace LugaStore.Application.Common.Settings;

public record RateLimitPolicyConfig
{
    public TimeSpan Window { get; init; }
    public int PermitLimit { get; init; }
    public int QueueLimit { get; init; }
    public QueueProcessingOrder QueueProcessingOrder { get; init; }
    public RateLimitPartition Partition { get; init; }
}
