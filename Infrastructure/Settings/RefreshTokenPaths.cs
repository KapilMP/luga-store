namespace LugaStore.Infrastructure.Settings;

public record RefreshTokenPaths : IRefreshTokenPaths
{
    public string CustomerRefreshPath { get; init; } = string.Empty;
    public string AdminRefreshPath { get; init; } = string.Empty;
    public string PartnerRefreshPath { get; init; } = string.Empty;
    public string PartnerManagerRefreshPath { get; init; } = string.Empty;
}
