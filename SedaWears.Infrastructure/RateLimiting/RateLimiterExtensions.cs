using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using SedaWears.Application.Common.Settings;

namespace SedaWears.Infrastructure.RateLimiting;

public static class RateLimiterExtensions
{
    public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
    {
        services.AddRateLimiter();
        services.AddOptions<RateLimiterOptions>()
            .Configure<IOptions<RateLimitingConfig>>((options, configOptions) =>
            {
                var config = configOptions.Value;
                options.RejectionStatusCode = config.RejectionStatusCode;

                foreach (var policyEntry in config.Policies)
                {
                    var policyName = policyEntry.Key;
                    var policyConfig = policyEntry.Value;

                    options.AddPolicy(policyName, context =>
                    {
                        var partitionKey = RateLimitPartitionKeyResolver.Resolve(context, policyConfig.Partition);

                        return System.Threading.RateLimiting.RateLimitPartition.GetFixedWindowLimiter(
                            partitionKey,
                            _ => new FixedWindowRateLimiterOptions
                            {
                                PermitLimit = policyConfig.PermitLimit,
                                Window = policyConfig.Window,
                                QueueLimit = policyConfig.QueueLimit,
                                QueueProcessingOrder = (System.Threading.RateLimiting.QueueProcessingOrder)policyConfig.QueueProcessingOrder
                            });
                    });
                }
            });

        return services;
    }
}
