using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SedaWears.Domain.Entities;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Settings;
using System.Web;

namespace SedaWears.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest;

public class ForgotPasswordValidator : AbstractValidator<ForgotPasswordCommand>
{
    public ForgotPasswordValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");
    }
}

public class ForgotPasswordHandler(
    UserManager<User> userManager,
    IEmailService emailService,
    IOriginContext originContext,
    AppConfig appConfig) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var role = originContext.CurrentRole;
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == role, ct);

        if (user == null) return;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/reset-password?email={user.Email}&token={HttpUtility.UrlEncode(token)}";

        await emailService.SendEmailAsync(
            user.Email!,
            "Reset Password",
            $"<p>To reset your password, click <a href='{url}'>here</a>.</p>");
    }
}
