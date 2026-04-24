using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Settings;
using SedaWears.Domain.Enums;
using SedaWears.Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;
using System.Web;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Users.Commands;

public record InviteOwnerCommand(string Email) : IRequest;

public record AdminInviteShopOwnerCommand(int ShopId, string Email) : IRequest;

public class InviteOwnerValidator : AbstractValidator<InviteOwnerCommand>
{
    public InviteOwnerValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("Enter valid email address").EmailAddress().WithMessage("Enter valid email address");
    }
}

public class AdminInviteShopOwnerValidator : AbstractValidator<AdminInviteShopOwnerCommand>
{
    public AdminInviteShopOwnerValidator()
    {
        RuleFor(x => x.ShopId)
            .GreaterThan(0).WithMessage("A valid shop identifier is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");
    }
}

public class InviteOwnerHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailService emailService,
    AppConfig appConfig) : IRequestHandler<InviteOwnerCommand>, IRequestHandler<AdminInviteShopOwnerCommand>
{
    public async Task Handle(InviteOwnerCommand request, CancellationToken ct)
    {
        await InviteInternal(null, request.Email, userManager, dbContext, emailService, appConfig, ct);
    }

    public async Task Handle(AdminInviteShopOwnerCommand request, CancellationToken ct)
    {
        await InviteInternal(request.ShopId, request.Email, userManager, dbContext, emailService, appConfig, ct);
    }

    internal static async Task InviteInternal(
        int? shopId,
        string email,
        UserManager<User> userManager,
        IApplicationDbContext dbContext,
        IEmailService emailService,
        AppConfig appConfig,
        CancellationToken ct)
    {
        string? shopName = null;
        if (shopId.HasValue)
        {
            var shop = await dbContext.Shops.FirstOrDefaultAsync(s => s.Id == shopId.Value, ct) ?? throw new NotFoundException("Shop not found.");
            shopName = shop.Name;
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Role == UserRole.Owner, ct);

        using var transaction = await dbContext.Database.BeginTransactionAsync(ct);
        try
        {
            if (user == null)
            {
                user = new User
                {
                    Email = email,
                    UserName = Guid.NewGuid().ToString(),
                    FirstName = "",
                    LastName = "",
                    IsActive = true,
                    Role = UserRole.Owner
                };

                var result = await userManager.CreateAsync(user);
                if (!result.Succeeded)
                    throw new BadRequestException(result.Errors.First().Description);
            }
            else if (!shopId.HasValue)
            {
                throw new BadRequestException("User is already an Owner.");
            }

            if (shopId.HasValue)
            {
                if (await dbContext.ShopOwners.AnyAsync(so => so.ShopId == shopId.Value && so.OwnerId == user.Id, ct))
                {
                    throw new BadRequestException("User is already an owner of this shop.");
                }

                dbContext.ShopOwners.Add(new ShopOwner { ShopId = shopId.Value, OwnerId = user.Id });
                await dbContext.SaveChangesAsync(ct);
            }

            await transaction.CommitAsync(ct);
        }
        catch { await transaction.RollbackAsync(ct); throw; }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}";

        var subject = shopName != null ? $"SedaWears Owner Invitation for {shopName}" : "SedaWears Owner Invitation";
        var body = shopName != null
            ? $"<p>You have been invited as an Owner for <b>{shopName}</b> on SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>"
            : $"<p>You have been invited as an Owner to SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>";

        await emailService.SendEmailAsync(user.Email!, subject, body);
    }
}
