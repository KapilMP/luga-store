using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Domain.Entities;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest;

public class ForgotPasswordHandler(
    UserManager<User> userManager,
    IEmailService emailService) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null) return;

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        await emailService.SendEmailAsync(
            user.Email!,
            "Reset Password",
            $"Token: {token}");
    }
}
