using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;

namespace LugaStore.Application.Features.Newsletter.Commands;

public record ConfirmUnsubscribeCommand(string Token) : IRequest;

public class ConfirmUnsubscribeCommandHandler(IApplicationDbContext context) : IRequestHandler<ConfirmUnsubscribeCommand>
{
    public async Task Handle(ConfirmUnsubscribeCommand request, CancellationToken cancellationToken)
    {
        var existing = await context.NewsletterSubscribers
            .FirstOrDefaultAsync(n => n.UnsubscribeToken == request.Token, cancellationToken)
            ?? throw new NotFoundError("Invalid or expired unsubscribe link.");

        existing.IsSubscribed = false;
        await context.SaveChangesAsync(cancellationToken);
    }
}
