using System.Web;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Common.Settings;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Invitations.Commands;

public record InvitePartnerManagerCommand(int PartnerId, string Email) : IRequest;

public class InvitePartnerManagerCommandHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailSender emailSender,
    AppConfig appConfig) :
    IRequestHandler<InvitePartnerManagerCommand>
{
    public async Task Handle(InvitePartnerManagerCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify Partner exists
        var partner = await userManager.FindByIdAsync(request.PartnerId.ToString()) ?? throw new NotFoundError("Partner not found.");
        if (!await userManager.IsInRoleAsync(partner, Roles.Partner)) throw new BadRequestError("User is not a partner.");

        var user = await userManager.FindByEmailAsync(request.Email);

        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            // 2. Create/Find Manager
            if (user != null)
            {
                if (!await userManager.IsInRoleAsync(user, Roles.PartnerManager))
                    await userManager.AddToRoleAsync(user, Roles.PartnerManager);
            }
            else
            {
                user = new User
                {
                    Email = request.Email,
                    UserName = Guid.NewGuid().ToString(),
                    IsActive = true
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded) throw new BadRequestError(result.Errors.First().Description);
                await userManager.AddToRoleAsync(user, Roles.PartnerManager);
            }

            // 3. Create mapping if not exists
            if (!await dbContext.PartnerManagers.AnyAsync(pm => pm.PartnerId == request.PartnerId && pm.ManagerId == user.Id, cancellationToken))
            {
                dbContext.PartnerManagers.Add(new PartnerManager { PartnerId = request.PartnerId, ManagerId = user.Id });
                await dbContext.SaveChangesAsync(cancellationToken);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        // 4. Send email
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={encodedToken}";
        
        await emailSender.SendEmailAsync(user.Email!, "LugaStore Manager Invitation", $"Click here to accept: {url}");
    }
}
