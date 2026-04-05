using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Options;
using LugaStore.Application.Common.Configurations;

namespace LugaStore.Infrastructure.Configurations;

public class ConfigureRateLimiterOptions(IOptions<RateLimitConfig> settings) : IConfigureOptions<RateLimiterOptions>
{

    public void Configure(RateLimiterOptions options)
    {
        var settingsValue = settings.Value;

        options.RejectionStatusCode = settingsValue.RejectionStatusCode;

        foreach (var policy in settingsValue.Policies)
        {
            options.AddFixedWindowLimiter(policy.Key, opt =>
            {
                opt.Window = TimeSpan.Parse(policy.Value.Window);
                opt.PermitLimit = policy.Value.PermitLimit;
                opt.QueueLimit = policy.Value.QueueLimit;
                opt.QueueProcessingOrder = Enum.Parse<QueueProcessingOrder>(policy.Value.QueueProcessingOrder);
            });
        }
    }
}
