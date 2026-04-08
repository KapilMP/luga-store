using MediatR;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Emails.Commands;

public record RetryEmailCommand(int LogId) : IRequest<bool>;

public class RetryEmailHandler(IApplicationDbContext dbContext, IPublishEndpoint publishEndpoint) : IRequestHandler<RetryEmailCommand, bool>
{
    public async Task<bool> Handle(RetryEmailCommand request, CancellationToken cancellationToken)
    {
        var log = await dbContext.EmailLogs.FirstOrDefaultAsync(l => l.Id == request.LogId, cancellationToken);
        if (log == null) return false;

        // Re-send the email through the existing background service by publishing the event again
        await publishEndpoint.Publish(new EmailSentEvent(log.To, log.Subject, log.Body), cancellationToken);

        log.Status = EmailStatus.Pending;
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
