namespace SedaWears.Application.Common.Settings;

public record RefreshTokenPathsConfig
{
    public string CustomerRefreshPath { get; init; } = string.Empty;
    public string AdminRefreshPath { get; init; } = string.Empty;
    public string OwnerRefreshPath { get; init; } = string.Empty;
    public string ManagerRefreshPath { get; init; } = string.Empty;
}
