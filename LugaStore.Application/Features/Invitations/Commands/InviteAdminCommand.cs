using System.Web;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Common.Settings;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Invitations.Commands;

public record InviteAdminCommand(string Email) : IRequest;

public class InviteAdminCommandHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailSender emailSender,
    AppConfig appConfig) :
    IRequestHandler<InviteAdminCommand>
{
    public async Task Handle(InviteAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        
        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            if (user != null)
            {
                if (await userManager.IsInRoleAsync(user, Roles.Admin)) throw new ConflictError("User is already an admin.");
                await userManager.AddToRoleAsync(user, Roles.Admin);
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
                
                await userManager.AddToRoleAsync(user, Roles.Admin);
            }

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw;
        }

        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        var encodedToken = HttpUtility.UrlEncode(token);
        var url = $"{appConfig.FrontendUrl}/accept-invitation?email={user.Email}&token={encodedToken}";
        
        await emailSender.SendEmailAsync(user.Email!, "LugaStore Admin Invitation", $"Click here to accept: {url}");
    }
}
