using MassTransit;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Infrastructure.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace LugaStore.Infrastructure.Messaging.Consumers;

public class EmailConsumer(
    ILogger<EmailConsumer> logger, 
    IEmailSettings settings,
    IApplicationDbContext dbContext) : IConsumer<EmailSentEvent>
{
    public async Task Consume(ConsumeContext<EmailSentEvent> context)
    {
        var message = context.Message;
        var messageId = context.MessageId?.ToString();

        // Check if log already exists (idempotency check for retries)
        var log = await dbContext.EmailLogs
            .FirstOrDefaultAsync(l => l.MessageId == messageId, CancellationToken.None);

        if (log == null)
        {
            log = new EmailLog
            {
                To = message.To,
                Subject = message.Subject,
                Body = message.Body,
                Status = EmailStatus.Pending,
                MessageId = messageId,
                SentCount = 0
            };
            dbContext.EmailLogs.Add(log);
        }

        log.LastAttemptAt = DateTime.UtcNow;
        await dbContext.SaveChangesAsync(CancellationToken.None);

        try
        {
            log.SentCount++;
            
            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(settings.FromName, settings.FromEmail));
            email.To.Add(MailboxAddress.Parse(message.To));
            email.Subject = message.Subject;
            
            var builder = new BodyBuilder { HtmlBody = message.Body };
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            
            var secureSocketOptions = settings.UseSsl ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls;
            await smtp.ConnectAsync(settings.Host, settings.Port, secureSocketOptions);

            if (!string.IsNullOrEmpty(settings.Username))
            {
                await smtp.AuthenticateAsync(settings.Username, settings.Password);
            }

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);

            log.Status = EmailStatus.Sent;
            log.ErrorMessage = null;
            await dbContext.SaveChangesAsync(CancellationToken.None);

            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("EMAIL SENT SUCCESSFULLY: LogId: {Id}, To: {To}, Attempt: {Attempt}", 
                    log.Id, message.To, log.SentCount);
            }
        }
        catch (Exception ex)
        {
            log.Status = EmailStatus.Failed;
            log.ErrorMessage = ex.ToString();
            await dbContext.SaveChangesAsync(CancellationToken.None);

            logger.LogError(ex, "FAILED TO SEND EMAIL: LogId: {Id}, To: {To}, Attempt: {Attempt}", 
                log.Id, message.To, log.SentCount);
            
            // Re-throw to trigger MassTransit retry
            throw;
        }
    }
}
