using MassTransit;
using Microsoft.Extensions.Logging;
using LugaStore.Application.Common.Models;

namespace LugaStore.Infrastructure.Messaging.Consumers;

public class EmailConsumer(ILogger<EmailConsumer> logger) : IConsumer<EmailSentEvent>
{
    public Task Consume(ConsumeContext<EmailSentEvent> context)
    {
        var message = context.Message;

        logger.LogWarning("EMAIL CONSUMED FROM RABBITMQ (SUCCESS): Subject: {Subject}",
            message.Subject);

        return Task.CompletedTask;
    }
}
