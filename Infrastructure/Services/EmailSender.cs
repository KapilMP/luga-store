using MassTransit;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Infrastructure.Services;

public class EmailSender(IPublishEndpoint publishEndpoint) : IEmailSender
{
    public async Task SendEmailAsync(string email, string subject, string message)
    {
        await publishEndpoint.Publish(new EmailSentEvent(email, subject, message));
    }
}
