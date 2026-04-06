using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Emails.Commands;

public record RetryEmailCommand(int LogId) : ICommand<bool>;
