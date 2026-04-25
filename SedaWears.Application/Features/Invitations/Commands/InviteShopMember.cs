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

namespace SedaWears.Application.Features.Invitations.Commands;

public record InviteShopMemberCommand(int ShopId, string Email, UserRole Role) : IRequest;

public class InviteShopMemberValidator : AbstractValidator<InviteShopMemberCommand>
{
    public InviteShopMemberValidator()
    {
        RuleFor(x => x.ShopId)
            .GreaterThan(0).WithMessage("A valid shop identifier is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");

        RuleFor(x => x.Role)
            .IsInEnum()
            .WithMessage("Invalid shop member role.");
    }
}

public class InviteShopMemberHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailService emailService,
    AppConfig appConfig) : IRequestHandler<InviteShopMemberCommand>
{
    public async Task Handle(InviteShopMemberCommand request, CancellationToken ct)
    {
        var shop = await dbContext.Shops.AsNoTracking().FirstOrDefaultAsync(s => s.Id == request.ShopId, ct)
            ?? throw new NotFoundException("Shop not found.");

        if (await dbContext.ShopMembers.AnyAsync(sm => sm.ShopId == request.ShopId && sm.User.Email == request.Email, ct))
        {
            throw new BadRequestException("Email is already in use.");
        }

        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == request.Role, ct);

        if (user == null)
        {
            user = new User
            {
                Email = request.Email,
                UserName = Guid.NewGuid().ToString(),
                FirstName = "",
                LastName = "",
                IsActive = true,
                Role = request.Role
            };
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);
        }

        dbContext.ShopMembers.Add(new ShopMember
        {
            ShopId = request.ShopId,
            UserId = user.Id,
            IsInvitationAccepted = false
        });
        await dbContext.SaveChangesAsync(ct);

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={HttpUtility.UrlEncode(token)}&shopId={request.ShopId}";

        var roleName = request.Role.ToString();
        await emailService.SendEmailAsync(
            user.Email!,
            $"SedaWears {roleName} Invitation for {shop.Name}",
            $"<p>You have been invited as a {roleName} for <b>{shop.Name}</b> on SedaWears.</p><p>Click <a href='{url}'>here</a> to accept the invitation and set your password.</p>");
    }
}
