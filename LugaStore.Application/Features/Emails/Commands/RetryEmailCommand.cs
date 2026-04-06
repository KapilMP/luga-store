using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Emails.Commands;

public record RetryEmailCommand(int LogId) : IRequest<bool>;

public class RetryEmailCommandHandler(IApplicationDbContext dbContext, IEmailSender emailSender) : IRequestHandler<RetryEmailCommand, bool>
{
    public async Task<bool> Handle(RetryEmailCommand request, CancellationToken cancellationToken)
    {
        var log = await dbContext.EmailLogs.FirstOrDefaultAsync(l => l.Id == request.LogId, cancellationToken);
        if (log == null) return false;

        // Re-send the email through the existing background service
        await emailSender.SendEmailAsync(log.To, log.Subject, log.Body);

        log.Status = EmailStatus.Pending;
        await dbContext.SaveChangesAsync(cancellationToken);

        return true;
    }
}
