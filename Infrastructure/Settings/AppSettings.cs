namespace LugaStore.Infrastructure.Settings;

public record AppSettings : IAppSettings
{
    public string FrontendUrl { get; init; } = string.Empty;
}
