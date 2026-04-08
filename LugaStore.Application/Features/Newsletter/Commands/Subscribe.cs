using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Newsletter.Commands;

public record SubscribeCommand(string Email) : IRequest;

public class SubscribeCommandHandler(IApplicationDbContext context) : IRequestHandler<SubscribeCommand>
{
    public async Task Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        var existing = await context.NewsletterSubscribers
            .FirstOrDefaultAsync(n => n.Email == request.Email, cancellationToken);

        if (existing != null)
        {
            if (existing.IsSubscribed) throw new ConflictError("You are already subscribed!");
            
            existing.IsSubscribed = true;
            existing.UnsubscribeToken = Guid.NewGuid().ToString("N");
        }
        else
        {
            context.NewsletterSubscribers.Add(new NewsletterSubscriber
            {
                Email = request.Email,
                IsSubscribed = true,
                UnsubscribeToken = Guid.NewGuid().ToString("N")
            });
        }

        await context.SaveChangesAsync(cancellationToken);
    }
}
