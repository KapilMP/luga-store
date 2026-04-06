using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Emails.Queries;

public class GetEmailLogsQueryHandler(IApplicationDbContext dbContext) : IQueryHandler<GetEmailLogsQuery, EmailLogsDto>
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
