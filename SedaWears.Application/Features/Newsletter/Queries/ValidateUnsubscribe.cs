using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Newsletter.Queries;

public record ValidateUnsubscribeQuery(string Token) : IRequest<string?>;

public class ValidateUnsubscribeQueryHandler(IApplicationDbContext context) : IRequestHandler<ValidateUnsubscribeQuery, string?>
{
    public async Task<string?> Handle(ValidateUnsubscribeQuery request, CancellationToken cancellationToken)
    {
        var entry = await context.NewsletterSubscribers.FirstOrDefaultAsync(n => n.UnsubscribeToken == request.Token, cancellationToken);
        return entry?.Email;
    }
}
