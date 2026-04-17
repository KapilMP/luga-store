namespace LugaStore.Application.Common.Settings;

public record ConnectionStringsConfig
{
    public string Postgres { get; init; } = string.Empty;
}
