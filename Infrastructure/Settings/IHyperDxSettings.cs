namespace LugaStore.Infrastructure.Settings;

public interface IHyperDxSettings
{
    string ApiKey { get; init; }
    string ServiceName { get; init; }
}
