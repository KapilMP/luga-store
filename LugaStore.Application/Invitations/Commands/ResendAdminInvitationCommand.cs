using System.Web;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Common.Configurations;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Invitations.Commands;

public record ResendAdminInvitationCommand(int UserId) : IRequest;

public class ResendAdminInvitationCommandHandler(
    UserManager<User> userManager,
    IEmailSender emailSender,
    AppConfig appConfig) :
    IRequestHandler<ResendAdminInvitationCommand>
{
    public async Task Handle(ResendAdminInvitationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Admin not found.");
        if (!await userManager.IsInRoleAsync(user, Roles.Admin)) throw new BadRequestError("User is not an admin.");
        if (user.EmailConfirmed) throw new BadRequestError("Invitation already accepted.");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={encodedToken}";
        
        await emailSender.SendEmailAsync(user.Email!, "LugaStore Admin Invitation (Resent)", $"Click here to accept: {url}");
    }
}
