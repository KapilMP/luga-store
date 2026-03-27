namespace LugaStore.Infrastructure.Settings;

public interface IOpeninarySettings
{
    string BaseUrl { get; init; }
    string ApiKey { get; init; }
}
