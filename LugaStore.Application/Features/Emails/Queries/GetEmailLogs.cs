using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Emails.Queries;

public record EmailLogsDto(List<EmailLogItemDto> Items, int TotalCount);
public record EmailLogItemDto(int Id, string To, string Subject, EmailStatus Status, DateTime CreatedAt, int SentCount, DateTime? LastAttemptAt);

public record GetEmailLogsQuery(int Page = 1, int PageSize = 20, EmailStatus? Status = null) : IRequest<EmailLogsDto>;

public class GetEmailLogsHandler(IApplicationDbContext dbContext) : IRequestHandler<GetEmailLogsQuery, EmailLogsDto>
{
    public async Task<EmailLogsDto> Handle(GetEmailLogsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.EmailLogs.AsNoTracking();

        if (request.Status.HasValue)
            query = query.Where(l => l.Status == request.Status.Value);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(l => l.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new EmailLogItemDto(l.Id, l.To, l.Subject, l.Status, l.CreatedAt, l.SentCount, l.LastAttemptAt))
            .ToListAsync(cancellationToken);

        return new EmailLogsDto(items, total);
    }
}
