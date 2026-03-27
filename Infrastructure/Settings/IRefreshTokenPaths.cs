namespace LugaStore.Infrastructure.Settings;

public interface IRefreshTokenPaths
{
    string CustomerRefreshPath { get; init; }
    string AdminRefreshPath { get; init; }
    string PartnerRefreshPath { get; init; }
    string PartnerManagerRefreshPath { get; init; }
}
