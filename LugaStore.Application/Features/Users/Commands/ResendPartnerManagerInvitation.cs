using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using MassTransit;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Settings;
using LugaStore.Domain.Entities;
using LugaStore.Application.Common.Models;
using System.Web;

namespace LugaStore.Application.Features.Users.Commands;

public record ResendPartnerManagerInvitationCommand(int ManagerId) : IRequest;

public class ResendPartnerManagerInvitationValidator : AbstractValidator<ResendPartnerManagerInvitationCommand>
{
    public ResendPartnerManagerInvitationValidator()
    {
        RuleFor(x => x.ManagerId).GreaterThan(0);
    }
}

public class ResendPartnerManagerInvitationHandler(
    UserManager<User> userManager,
    IPublishEndpoint publishEndpoint,
    AppConfig appConfig) : IRequestHandler<ResendPartnerManagerInvitationCommand>
{
    public async Task Handle(ResendPartnerManagerInvitationCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.ManagerId.ToString()) ?? throw new NotFoundError("User not found.");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}";

        await publishEndpoint.Publish(new EmailSentEvent(
            user.Email!,
            "LugaStore Manager Invitation Re-sent",
            $"Click here to accept: {url}"), ct);
    }
}
