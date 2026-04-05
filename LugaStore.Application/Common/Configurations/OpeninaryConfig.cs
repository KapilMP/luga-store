namespace LugaStore.Application.Common.Configurations;

public record OpeninaryConfig
{
    public string BaseUrl { get; init; } = string.Empty;
    public string ApiKey { get; init; } = string.Empty;
}
