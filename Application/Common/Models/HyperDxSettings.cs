using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Common.Models;

public record HyperDxSettings : IHyperDxSettings
{
    public string ApiKey { get; init; } = string.Empty;
    public string ServiceName { get; init; } = string.Empty;
}
