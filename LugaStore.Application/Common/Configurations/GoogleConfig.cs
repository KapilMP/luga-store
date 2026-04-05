namespace LugaStore.Application.Common.Configurations;

public record GoogleConfig
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
}
