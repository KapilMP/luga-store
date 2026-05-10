using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using RedisRateLimiting;
using SedaWears.Application.Common.Settings;
using StackExchange.Redis;

namespace SedaWears.Infrastructure.RateLimiting;

internal sealed class ConfigureRateLimiterOptions(IOptions<RateLimitingConfig> configOptions, IConnectionMultiplexer redis) : IConfigureOptions<RateLimiterOptions>
{
    private readonly RateLimitingConfig _config = configOptions.Value;

    public void Configure(RateLimiterOptions options)
    {
        options.RejectionStatusCode = _config.RejectionStatusCode;

        foreach (var policyEntry in _config.Policies)
        {
            var policyName = policyEntry.Key;
            var policyConfig = policyEntry.Value;

            options.AddPolicy(policyName, context =>
            {
                var rawPartitionKey = RateLimitPartitionKeyResolver.Resolve(context, policyConfig.PartitionType);
                var scopedPartitionKey = $"rl:{policyName}:{rawPartitionKey}";

                return RedisRateLimitPartition.GetFixedWindowRateLimiter(
                   scopedPartitionKey,
                    _ => new RedisFixedWindowRateLimiterOptions
                    {
                        ConnectionMultiplexerFactory = () => redis,
                        PermitLimit = policyConfig.PermitLimit,
                        Window = policyConfig.Window,
                    });
            });
        }
    }
}

public static class RateLimiterExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter();
        services.AddSingleton<IConfigureOptions<RateLimiterOptions>, ConfigureRateLimiterOptions>();

        return services;
    }
}
