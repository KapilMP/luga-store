using System.Collections.Generic;

namespace LugaStore.Application.Common.Configurations;

public record AppConfig
{
    public string FrontendUrl { get; init; } = string.Empty;
    public CorsConfig Cors { get; init; } = new();
}

public record CorsConfig
{
    public List<string> AllowedOrigins { get; init; } = [];
}
