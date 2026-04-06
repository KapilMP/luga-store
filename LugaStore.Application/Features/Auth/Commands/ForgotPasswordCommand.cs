using System.Web;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;
using LugaStore.Application.Common.Settings;

namespace LugaStore.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest;

public class ForgotPasswordCommandHandler(
    UserManager<User> userManager,
    IEmailSender emailSender,
    AppConfig appConfig) :
    IRequestHandler<ForgotPasswordCommand>
{
    private string FrontendUrl => appConfig.FrontendUrl;

    public async Task Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null) return;
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var resetUrl = $"{FrontendUrl}/reset-password?email={request.Email}&token={encodedToken}";
        await emailSender.SendEmailAsync(request.Email, "Reset Password", $"Reset your password by clicking here: {resetUrl}");
    }
}
