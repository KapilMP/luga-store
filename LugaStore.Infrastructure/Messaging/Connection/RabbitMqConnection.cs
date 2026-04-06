using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using LugaStore.Application.Common.Settings;
using LugaStore.Infrastructure.Messaging.Consumers;
using LugaStore.Infrastructure.Messaging;

namespace LugaStore.Infrastructure.Messaging.Connection;

public static class RabbitMqConnection
{
    public static IServiceCollection AddMessagingByRabbitMq(this IServiceCollection services)
    {
        services.AddMassTransit(x =>
        {
            x.SetKebabCaseEndpointNameFormatter();
            
            // Add all consumers from this assembly
            x.AddConsumer<EmailConsumer>();

            x.UsingRabbitMq((context, cfg) =>
            {
                var config = context.GetRequiredService<ConnectionStringsConfig>();
                
                cfg.Host(config.RabbitMq);

                // Configure Endpoints using constants
                cfg.ReceiveEndpoint(MessagingConstants.EmailQueue, e =>
                {
                    e.ConfigureConsumer<EmailConsumer>(context);
                    
                    e.UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)));
                    e.UseScheduledRedelivery(r => r.Intervals(
                        TimeSpan.FromMinutes(5), 
                        TimeSpan.FromMinutes(15), 
                        TimeSpan.FromMinutes(30)));
                });
            });
        });

        return services;
    }
}
