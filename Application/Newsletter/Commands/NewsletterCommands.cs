using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.NewsletterFeature.Commands;

public record SubscribeCommand(string Email) : IRequest<string>;

public class SubscribeCommandHandler(IApplicationDbContext context) : IRequestHandler<SubscribeCommand, string>
{
    public async Task<string> Handle(SubscribeCommand request, CancellationToken cancellationToken)
    {
        var existing = await context.Newsletters.FirstOrDefaultAsync(n => n.Email == request.Email, cancellationToken);

        if (existing != null)
        {
            if (existing.IsSubscribed) return "already_subscribed";
            existing.IsSubscribed = true;
            existing.UnsubscribeToken = Guid.NewGuid().ToString("N");
            await context.SaveChangesAsync(cancellationToken);
            return "reactivated";
        }

        context.Newsletters.Add(new LugaStore.Domain.Entities.Newsletter
        {
            Email = request.Email,
            IsSubscribed = true,
            UnsubscribeToken = Guid.NewGuid().ToString("N")
        });
        await context.SaveChangesAsync(cancellationToken);
        return "subscribed";
    }
}

public record ConfirmUnsubscribeCommand(string Token) : IRequest<bool>;

public class ConfirmUnsubscribeCommandHandler(IApplicationDbContext context) : IRequestHandler<ConfirmUnsubscribeCommand, bool>
{
    public async Task<bool> Handle(ConfirmUnsubscribeCommand request, CancellationToken cancellationToken)
    {
        var existing = await context.Newsletters.FirstOrDefaultAsync(n => n.UnsubscribeToken == request.Token, cancellationToken);
        if (existing == null) return false;

        existing.IsSubscribed = false;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}

public record UnsubscribeCommand(string Email) : IRequest<bool>;

public class UnsubscribeCommandHandler(IApplicationDbContext context) : IRequestHandler<UnsubscribeCommand, bool>
{
    public async Task<bool> Handle(UnsubscribeCommand request, CancellationToken cancellationToken)
    {
        var existing = await context.Newsletters.FirstOrDefaultAsync(n => n.Email == request.Email, cancellationToken);
        if (existing == null) return false;

        existing.IsSubscribed = false;
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
