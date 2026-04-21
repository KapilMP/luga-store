using Resend;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Settings;

namespace SedaWears.Infrastructure.Services;

public class EmailService(IResend resend, EmailConfig config) : IEmailService
{
    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var message = new EmailMessage();
        message.From = $"{config.FromName} <{config.FromEmail}>";
        message.To.Add(to);
        message.Subject = subject;
        message.HtmlBody = body;

        await resend.EmailSendAsync(message);
    }
}
