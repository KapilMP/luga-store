using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;

namespace LugaStore.Application.Features.Newsletter.Commands;

public record UnsubscribeCommand(string Email) : IRequest;

public class UnsubscribeCommandHandler(IApplicationDbContext context) : IRequestHandler<UnsubscribeCommand>
{
    public async Task Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
    {
        var existing = await context.NewsletterSubscribers
            .FirstOrDefaultAsync(n => n.Email == request.Email, cancellationToken)
            ?? throw new NotFoundError("Subscriber with this email was not found.");

        existing.IsSubscribed = false;
        await context.SaveChangesAsync(cancellationToken);
    }
}
