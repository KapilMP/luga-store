using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Settings;
using System.Web;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Users.Commands;

public record ResendManagerInvitationCommand(int ManagerId) : IRequest;

public class ResendManagerInvitationValidator : AbstractValidator<ResendManagerInvitationCommand>
{
    public ResendManagerInvitationValidator()
    {
        RuleFor(x => x.ManagerId).GreaterThan(0);
    }
}

public class ResendManagerInvitationHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailService emailService,
    ICurrentUser currentUser,
    AppConfig appConfig) : IRequestHandler<ResendManagerInvitationCommand>
{
    public async Task Handle(ResendManagerInvitationCommand request, CancellationToken ct)
    {
        var shopId = currentUser.ShopId ?? throw new UnauthorizedAccessException("Shop context missing. Use X-Shop-ID.");
        
        var sm = await dbContext.ShopManagers
            .Include(x => x.Shop)
            .FirstOrDefaultAsync(x => x.ShopId == shopId && x.ManagerId == request.ManagerId, ct)
            ?? throw new NotFoundException("Shop Manager linkage not found");

        var user = await userManager.FindByIdAsync(request.ManagerId.ToString()) ?? throw new NotFoundException("User not found");
        
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}";

        await emailService.SendEmailAsync(
            user.Email!,
            "SedaWears Manager Invitation Resend",
            $"<p>Your invitation for {sm.Shop.Name} has been resent.</p><p>Click <a href='{url}'>here</a> to accept and set your password.</p>");
    }
}
