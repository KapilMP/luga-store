namespace LugaStore.Application.Common.Configurations;

public record ConnectionStringsConfig
{
    public string Postgres { get; init; } = string.Empty;
    public string RabbitMq { get; init; } = string.Empty;
}
