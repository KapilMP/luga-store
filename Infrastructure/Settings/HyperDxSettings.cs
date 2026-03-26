namespace LugaStore.Infrastructure.Settings;

public record HyperDxSettings : IHyperDxSettings
{
    public string ApiKey { get; init; } = string.Empty;
    public string ServiceName { get; init; } = string.Empty;
}
