using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Common.Models;

public record GoogleSettings : IGoogleSettings
{
    public string ClientId { get; init; } = string.Empty;
    public string ClientSecret { get; init; } = string.Empty;
}
