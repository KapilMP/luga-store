namespace LugaStore.Application.Common.Models;

public record EmailSentEvent(string Email, string Subject, string Message);
