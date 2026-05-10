using Microsoft.AspNetCore.Http;
using SedaWears.Application.Common.Settings;

namespace SedaWears.Infrastructure.RateLimiting;

public static class RateLimitPartitionKeyResolver
{
    public static string Resolve(HttpContext context, RateLimitPartitionType partitionType)
    {
        return partitionType switch
        {
            RateLimitPartitionType.User =>
                context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            
            RateLimitPartitionType.IP =>
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            
            RateLimitPartitionType.Device =>
                context.Request.Headers["User-Agent"].ToString(),
            
            RateLimitPartitionType.Global => "global",
            
            _ => "global"
        };
    }
}
