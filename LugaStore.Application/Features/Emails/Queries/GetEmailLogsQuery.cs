using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Emails.Queries;

public record GetEmailLogsQuery(int Page = 1, int PageSize = 20, EmailStatus? Status = null) : IQuery<EmailLogsDto>;

public record EmailLogsDto(List<EmailLogItemDto> Items, int TotalCount);
public record EmailLogItemDto(int Id, string To, string Subject, EmailStatus Status, DateTime CreatedAt, int SentCount, DateTime? LastAttemptAt);
