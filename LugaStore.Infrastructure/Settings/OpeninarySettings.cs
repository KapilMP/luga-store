namespace LugaStore.Infrastructure.Settings;

public record OpeninarySettings : IOpeninarySettings
{
    public string BaseUrl { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
}
