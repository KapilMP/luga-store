using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Settings;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Web;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Users.Commands;

public record InviteManagerCommand(string Email) : IRequest;

public record AdminInviteShopManagerCommand(int ShopId, string Email) : IRequest;

public class InviteManagerValidator : AbstractValidator<InviteManagerCommand>
{
    public InviteManagerValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class AdminInviteShopManagerValidator : AbstractValidator<AdminInviteShopManagerCommand>
{
    public AdminInviteShopManagerValidator()
    {
        RuleFor(x => x.ShopId).GreaterThan(0);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}

public class InviteManagerHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailService emailService,
    AppConfig appConfig,
    ICurrentUser currentUser) : IRequestHandler<InviteManagerCommand>
{
    public async Task Handle(InviteManagerCommand request, CancellationToken ct)
    {
        var shopId = currentUser.ShopId ?? throw new UnauthorizedAccessException("Shop context missing. Use X-Shop-ID.");
        await InviteInternal(shopId, request.Email, userManager, dbContext, emailService, appConfig, ct);
    }

    internal static async Task InviteInternal(
        int shopId, 
        string email, 
        UserManager<User> userManager, 
        IApplicationDbContext dbContext,
        IEmailService emailService,
        AppConfig appConfig,
        CancellationToken ct)
    {
        var shop = await dbContext.Shops.FirstOrDefaultAsync(s => s.Id == shopId, ct) ?? throw new NotFoundException("Shop not found.");

        var user = await userManager.FindByEmailAsync(email);
        using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            if (user != null)
            {
                if (user.Role != UserRole.Manager)
                {
                    user.Role = UserRole.Manager;
                    await userManager.UpdateAsync(user);
                }
            }
            else
            {
                user = new User 
                { 
                    Email = email, 
                    UserName = Guid.NewGuid().ToString(), 
                    FirstName = "",
                    LastName = "",
                    IsActive = true,
                    Role = UserRole.Manager
                };
                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);
            }

            if (!await dbContext.ShopManagers.AnyAsync(sm => sm.ShopId == shopId && sm.ManagerId == user.Id, ct))
            {
                dbContext.ShopManagers.Add(new ShopManager { ShopId = shopId, ManagerId = user.Id });
                await dbContext.SaveChangesAsync(ct);
            }
            await transaction.CommitAsync(ct);
        }
        catch { await transaction.RollbackAsync(ct); throw; }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}";

        await emailService.SendEmailAsync(
            user.Email!,
            "SedaWears Manager Invitation",
            $"<p>You have been invited as a Manager for {shop.Name} on SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>");
    }
}

public class AdminInviteShopManagerHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailService emailService,
    AppConfig appConfig) : IRequestHandler<AdminInviteShopManagerCommand>
{
    public async Task Handle(AdminInviteShopManagerCommand request, CancellationToken ct)
    {
        await InviteManagerHandler.InviteInternal(request.ShopId, request.Email, userManager, dbContext, emailService, appConfig, ct);
    }
}
