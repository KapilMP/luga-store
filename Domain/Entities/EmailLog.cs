namespace LugaStore.Domain.Entities;

public enum EmailStatus
{
    Pending,
    Sent,
    Failed
}

public class EmailLog
{
    public int Id { get; set; }
    public string To { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public EmailStatus Status { get; set; } = EmailStatus.Pending;
    public string? ErrorMessage { get; set; }
    public string? MessageId { get; set; }
    public int SentCount { get; set; } = 0;
    public DateTime? LastAttemptAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
