using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Invitations.Commands;

public record ResendPartnerInvitationCommand(int PartnerId) : IRequest;

public class ResendPartnerInvitationCommandHandler(
    UserManager<User> userManager,
    IEmailSender emailSender) : 
    IRequestHandler<ResendPartnerInvitationCommand>
{
    public async Task Handle(ResendPartnerInvitationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.PartnerId.ToString());
        if (user == null || !await userManager.IsInRoleAsync(user, Roles.Partner) || user.EmailConfirmed)
            throw new NotFoundError("Partner not found or already verified.");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        await emailSender.SendEmailAsync(user.Email!, "Verify your Luga Store Partner Account", $"Your token: {token}");
    }
}
