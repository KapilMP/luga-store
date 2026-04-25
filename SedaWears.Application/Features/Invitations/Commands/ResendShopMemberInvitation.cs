using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Settings;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using System.Web;

namespace SedaWears.Application.Features.Invitations.Commands;

public record ResendShopMemberInvitationCommand(int ShopId, int UserId, UserRole Role) : IRequest;

public class ResendShopMemberInvitationHandler(
    IApplicationDbContext dbContext,
    UserManager<User> userManager,
    IEmailService emailService,
    AppConfig appConfig) : IRequestHandler<ResendShopMemberInvitationCommand>
{
    public async Task Handle(ResendShopMemberInvitationCommand request, CancellationToken ct)
    {
        var shop = await dbContext.Shops
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == request.ShopId, ct) ?? throw new NotFoundException("Shop not found.");

        var member = await dbContext.ShopMembers
            .Include(sm => sm.User)
            .FirstOrDefaultAsync(sm => sm.ShopId == request.ShopId && sm.UserId == request.UserId && sm.User.Role == request.Role, ct) 
            ?? throw new NotFoundException($"Shop {request.Role.ToString().ToLower()} not found.");

        if (member.IsInvitationAccepted)
        {
            throw new BadRequestException("Invitation already accepted.");
        }

        var user = member.User;
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}&shopId={request.ShopId}";

        var roleName = request.Role.ToString();
        var subject = $"SedaWears {roleName} Invitation for {shop.Name}";
        var body = $"<p>You have been invited as a {roleName} for <b>{shop.Name}</b> on SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>";

        await emailService.SendEmailAsync(user.Email!, subject, body);
    }
}
