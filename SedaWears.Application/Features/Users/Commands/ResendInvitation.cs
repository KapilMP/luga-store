using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Settings;
using System.Web;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users.Commands;

public record ResendInvitationCommand(int UserId, UserRole role) : IRequest;

public class ResendInvitationValidator : AbstractValidator<ResendInvitationCommand>
{
    public ResendInvitationValidator()
    {
        RuleFor(x => x.UserId)
            .GreaterThan(0).WithMessage("A valid user identifier is required.");
    }
}

public class ResendInvitationHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailService emailService,
    ICurrentUser currentUser,
    AppConfig appConfig) : IRequestHandler<ResendInvitationCommand>
{
    public async Task Handle(ResendInvitationCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.Role == request.role, ct)
            ?? throw new NotFoundException("User not found");

        if (user.EmailConfirmed)
            throw new BadRequestException("This user has already accepted their invitation.");


        if (currentUser.Role == UserRole.Owner)
        {
            var shopId = currentUser.ShopId ?? throw new UnauthorizedAccessException("Shop context missing.");
            var isManagerInShop = await dbContext.ShopManagers
                .AnyAsync(sm => sm.ShopId == shopId && sm.ManagerId == user.Id, ct);

            if (!isManagerInShop || user.Role != UserRole.Manager)
            {
                throw new ForbiddenException("You can only resend invitations for managers in your shop.");
            }
        }
        else if (currentUser.Role != UserRole.Admin)
        {
            throw new ForbiddenException("You are not authorized to resend invitations.");
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}";

        string subject;
        string body;

        switch (user.Role)
        {
            case UserRole.Admin:
                subject = "SedaWears Admin Invitation";
                body = $"<p>You have been invited as an Admin to SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>";
                break;
            case UserRole.Owner:
                subject = "SedaWears Owner Invitation";
                body = $"<p>You have been invited as an Owner to SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>";
                break;
            case UserRole.Manager:
                var shop = await dbContext.ShopManagers
                    .Where(sm => sm.ManagerId == user.Id)
                    .Select(sm => sm.Shop)
                    .FirstOrDefaultAsync(ct);

                subject = "SedaWears Manager Invitation";
                body = shop != null
                    ? $"<p>You have been invited as a Manager for {shop.Name} on SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>"
                    : $"<p>You have been invited as a Manager on SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>";
                break;
            default:
                throw new BadRequestException("Cannot resend invitation for this user role.");
        }

        await emailService.SendEmailAsync(user.Email!, subject, body);
    }
}
