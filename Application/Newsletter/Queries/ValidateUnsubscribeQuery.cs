using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.NewsletterFeature.Queries;

public record ValidateUnsubscribeQuery(string Token) : IRequest<string?>;

public class ValidateUnsubscribeQueryHandler(IApplicationDbContext context) : IRequestHandler<ValidateUnsubscribeQuery, string?>
{
    public async Task<string?> Handle(ValidateUnsubscribeQuery request, CancellationToken cancellationToken)
    {
        var entry = await context.Newsletters.FirstOrDefaultAsync(n => n.UnsubscribeToken == request.Token, cancellationToken);
        return entry?.Email;
    }
}
