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

public record ResendAdminInvitationCommand(int UserId) : IRequest;

public class ResendAdminInvitationHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailService emailService,
    AppConfig appConfig) : IRequestHandler<ResendAdminInvitationCommand>
{
    public async Task Handle(ResendAdminInvitationCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.Role == UserRole.Admin, ct)
            ?? throw new NotFoundException("Admin user not found.");

        if (user.EmailConfirmed)
            throw new BadRequestException("This admin has already accepted their invitation.");

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}&role={nameof(UserRole.Admin)}";
        
        var subject = "SedaWears Admin Invitation";
        var body = $"<p>You have been invited as an Admin to SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>";

        await emailService.SendEmailAsync(user.Email!, subject, body);
    }
}
