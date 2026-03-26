namespace LugaStore.Infrastructure.Settings;

public interface IGoogleSettings
{
    string ClientId { get; init; }
    string ClientSecret { get; init; }
}
