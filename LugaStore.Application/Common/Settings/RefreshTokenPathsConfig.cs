namespace LugaStore.Application.Common.Settings;

public record RefreshTokenPathsConfig
{
    public string CustomerRefreshPath { get; init; } = string.Empty;
    public string AdminRefreshPath { get; init; } = string.Empty;
    public string PartnerRefreshPath { get; init; } = string.Empty;
    public string PartnerManagerRefreshPath { get; init; } = string.Empty;
}
