namespace LugaStore.Application.Common.Interfaces;

public interface IGoogleSettings
{
    string ClientId { get; init; }
    string ClientSecret { get; init; }
}
