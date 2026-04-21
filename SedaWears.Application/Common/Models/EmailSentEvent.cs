namespace SedaWears.Application.Common.Models;

public record EmailSentEvent(string To, string Subject, string Body);
