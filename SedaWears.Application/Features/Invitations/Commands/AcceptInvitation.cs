using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Invitations.Commands;

public record AcceptInvitationCommand(
    int? ShopId,
    string Email,
    string Token,
    string FirstName,
    string LastName,
    string Password) : IRequest;

public class AcceptInvitationHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IOriginContext originContext) : IRequestHandler<AcceptInvitationCommand>
{
    public async Task Handle(AcceptInvitationCommand request, CancellationToken ct)
    {
        if (originContext.CurrentRole == UserRole.Admin)
        {
            var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == UserRole.Admin, ct)
            ?? throw new NotFoundException("Admin user not found.");

            if (user.IsAdminInvitationAccepted == true)
                throw new BadRequestException("Invitation already accepted.");

            await VerifyAndAcceptUser(user, request);
            user.IsAdminInvitationAccepted = true;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.First().Description);

            await userManager.UpdateSecurityStampAsync(user);
        }
        else if (originContext.CurrentRole is UserRole.Owner or UserRole.Manager)
        {
            var shopId = request.ShopId ?? throw new BadRequestException("Shop identifier is required.");

            var shop = await dbContext.Shops
                .Include(s => s.Members)
                .ThenInclude(m => m.User)
                .FirstOrDefaultAsync(s => s.Id == shopId, ct) ?? throw new NotFoundException("Shop not found.");

            var membership = shop.Members
                .FirstOrDefault(m => m.User.Email == request.Email) ?? throw new NotFoundException("Invitation not found.");

            if (membership.IsInvitationAccepted)
                throw new BadRequestException("Invitation already accepted.");

            var user = membership.User;
            await VerifyAndAcceptUser(user, request);

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
                throw new BadRequestException(result.Errors.First().Description);

            membership.IsInvitationAccepted = true;
            membership.CreatedAt = DateTime.UtcNow;

            await dbContext.SaveChangesAsync(ct);
            await userManager.UpdateSecurityStampAsync(user);
        }
        else
        {
            throw new BadRequestException("Role not supported for invitation acceptance.");
        }
    }
    private async Task VerifyAndAcceptUser(User user, AcceptInvitationCommand request)
    {
        var isValid = await userManager.VerifyUserTokenAsync(user, userManager.Options.Tokens.EmailConfirmationTokenProvider, "EmailConfirmation", request.Token);
        if (!isValid)
            throw new BadRequestException("Invalid or expired invitation token.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.EmailConfirmed = true;

        if (!await userManager.HasPasswordAsync(user))
        {
            var passwordResult = await userManager.AddPasswordAsync(user, request.Password);
            if (!passwordResult.Succeeded)
                throw new BadRequestException(passwordResult.Errors.First().Description);
        }
    }
}
