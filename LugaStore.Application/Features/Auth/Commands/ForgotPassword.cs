using LugaStore.Application.Features.Users.Models;
using MediatR;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Auth.Commands;

public record ForgotPasswordCommand(string Email) : IRequest;

public class ForgotPasswordHandler(
    UserManager<User> userManager,
    IPublishEndpoint publishEndpoint) : IRequestHandler<ForgotPasswordCommand>
{
    public async Task Handle(ForgotPasswordCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null) return;
        
        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        
        await publishEndpoint.Publish(new EmailSentEvent(
            user.Email!, 
            "Reset Password", 
            $"Token: {token}"), ct);
    }
}
