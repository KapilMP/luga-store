using LugaStore.Application.Common.Settings;
using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;
using System.Web;

namespace LugaStore.Application.Features.Users.Commands;

public record InvitePartnerManagerCommand(string Email) : IRequest;

public record AdminInvitePartnerManagerCommand(int PartnerId, string Email) : IRequest;

public class InvitePartnerManagerValidator : AbstractValidator<InvitePartnerManagerCommand>
{
    public InvitePartnerManagerValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class AdminInvitePartnerManagerValidator : AbstractValidator<AdminInvitePartnerManagerCommand>
{
    public AdminInvitePartnerManagerValidator()
    {
        RuleFor(x => x.PartnerId).GreaterThan(0);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class InvitePartnerManagerHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailService emailService,
    AppConfig appConfig,
    ICurrentUser currentUser) : IRequestHandler<InvitePartnerManagerCommand>
{
    public async Task Handle(InvitePartnerManagerCommand request, CancellationToken ct)
    {
        var partnerId = currentUser.Id!.Value;
        await InviteInternal(partnerId, request.Email, userManager, dbContext, emailService, appConfig, ct);
    }

    internal static async Task InviteInternal(
        int partnerId, 
        string email, 
        UserManager<User> userManager, 
        IApplicationDbContext dbContext,
        IEmailService emailService,
        AppConfig appConfig,
        CancellationToken ct)
    {
        var partner = await userManager.FindByIdAsync(partnerId.ToString()) ?? throw new NotFoundError("Partner not found.");
        if (!await userManager.IsInRoleAsync(partner, Roles.Partner)) throw new BadRequestError("User is not a partner.");

        var user = await userManager.FindByEmailAsync(email);
        using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            if (user != null)
            {
                if (!await userManager.IsInRoleAsync(user, Roles.PartnerManager))
                    await userManager.AddToRoleAsync(user, Roles.PartnerManager);
            }
            else
            {
                user = new User { Email = email, UserName = Guid.NewGuid().ToString(), IsActive = true };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded) throw new BadRequestError(result.Errors.First().Description);
                await userManager.AddToRoleAsync(user, Roles.PartnerManager);
            }

            if (!await dbContext.PartnerManagers.AnyAsync(pm => pm.PartnerId == partnerId && pm.ManagerId == user.Id, ct))
            {
                dbContext.PartnerManagers.Add(new PartnerManager { PartnerId = partnerId, ManagerId = user.Id });
                await dbContext.SaveChangesAsync(ct);
            }
            await transaction.CommitAsync(ct);
        }
        catch { await transaction.RollbackAsync(ct); throw; }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}";

        await emailService.SendEmailAsync(
            user.Email!,
            "LugaStore Manager Invitation",
            $"Click here to accept: {url}");
    }
}

public class AdminInvitePartnerManagerHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailService emailService,
    AppConfig appConfig) : IRequestHandler<AdminInvitePartnerManagerCommand>
{
    public async Task Handle(AdminInvitePartnerManagerCommand request, CancellationToken ct)
    {
        await InvitePartnerManagerHandler.InviteInternal(request.PartnerId, request.Email, userManager, dbContext, emailService, appConfig, ct);
    }
}
