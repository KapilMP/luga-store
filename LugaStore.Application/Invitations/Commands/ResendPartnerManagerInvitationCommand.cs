using System.Web;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Configurations;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Invitations.Commands;

public record ResendPartnerManagerInvitationCommand(int ManagerId) : IRequest;

public class ResendPartnerManagerInvitationCommandHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailSender emailSender,
    AppConfig appConfig) :
    IRequestHandler<ResendPartnerManagerInvitationCommand>
{
    public async Task Handle(ResendPartnerManagerInvitationCommand request, CancellationToken cancellationToken)
    {
        var manager = await userManager.FindByIdAsync(request.ManagerId.ToString()) ?? throw new NotFoundError("Manager not found.");
        
        var isPartnerManager = await dbContext.PartnerManagers.AnyAsync(pm => pm.ManagerId == request.ManagerId, cancellationToken);
        if (!isPartnerManager) throw new BadRequestError("User is not a partner manager.");
        
        if (manager.EmailConfirmed) throw new BadRequestError("Invitation already accepted.");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(manager);
        var encodedToken = HttpUtility.UrlEncode(token);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={manager.Email}&token={encodedToken}";
        
        await emailSender.SendEmailAsync(manager.Email!, "LugaStore Partner Manager Invitation (Resent)", $"Click here to accept: {url}");
    }
}
