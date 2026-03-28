namespace LugaStore.Infrastructure.Settings;

public record GoogleSettings : IGoogleSettings
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
}
