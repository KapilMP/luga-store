using Microsoft.AspNetCore.Http;
using SedaWears.Application.Common.Settings;

namespace SedaWears.Infrastructure.RateLimiting;

public static class RateLimitPartitionKeyResolver
{
    public static string Resolve(HttpContext context, RateLimitPartition partition)
    {
        return partition switch
        {
            RateLimitPartition.User =>
                context.User.Identity?.Name ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            
            RateLimitPartition.IP =>
                context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            
            RateLimitPartition.Device =>
                context.Request.Headers["User-Agent"].ToString(),
            
            RateLimitPartition.Global => "global",
            
            _ => "global"
        };
    }
}
