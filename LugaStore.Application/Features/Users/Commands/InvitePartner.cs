using LugaStore.Application.Common.Settings;
using LugaStore.Application.Features.Users.Models;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using MassTransit;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;
using System.Web;

namespace LugaStore.Application.Features.Users.Commands;

public record InvitePartnerManagerCommand(int PartnerId, string Email) : IRequest;

public class InvitePartnerManagerValidator : AbstractValidator<InvitePartnerManagerCommand>
{
    public InvitePartnerManagerValidator()
    {
        RuleFor(x => x.PartnerId).GreaterThan(0);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class InvitePartnerManagerHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IPublishEndpoint publishEndpoint,
    AppConfig appConfig) : IRequestHandler<InvitePartnerManagerCommand>
{
    public async Task Handle(InvitePartnerManagerCommand request, CancellationToken ct)
    {
        var partner = await userManager.FindByIdAsync(request.PartnerId.ToString()) ?? throw new NotFoundError("Partner not found.");
        if (!await userManager.IsInRoleAsync(partner, Roles.Partner)) throw new BadRequestError("User is not a partner.");

        var user = await userManager.FindByEmailAsync(request.Email);
        using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try {
            if (user != null) {
                if (!await userManager.IsInRoleAsync(user, Roles.PartnerManager))
                    await userManager.AddToRoleAsync(user, Roles.PartnerManager);
            } else {
                user = new User { Email = request.Email, UserName = Guid.NewGuid().ToString(), IsActive = true };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded) throw new BadRequestError(result.Errors.First().Description);
                await userManager.AddToRoleAsync(user, Roles.PartnerManager);
            }

            if (!await dbContext.PartnerManagers.AnyAsync(pm => pm.PartnerId == request.PartnerId && pm.ManagerId == user.Id, ct)) {
                dbContext.PartnerManagers.Add(new PartnerManager { PartnerId = request.PartnerId, ManagerId = user.Id });
                await dbContext.SaveChangesAsync(ct);
            }
            await transaction.CommitAsync(ct);
        } catch { await transaction.RollbackAsync(ct); throw; }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}";
        
        await publishEndpoint.Publish(new EmailSentEvent(
            user.Email!, 
            "LugaStore Manager Invitation", 
            $"Click here to accept: {url}"), ct);
    }
}
