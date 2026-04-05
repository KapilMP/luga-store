using System.Web;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Common.Configurations;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Invitations.Commands;

public record InvitePartnerCommand(string Email) : IRequest;

public class InvitePartnerCommandHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IEmailSender emailSender,
    AppConfig appConfig) :
    IRequestHandler<InvitePartnerCommand>
{
    public async Task Handle(InvitePartnerCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        
        using var transaction = await dbContext.Database.BeginTransactionAsync(cancellationToken);
        
        try
        {
            if (user != null)
            {
                if (await userManager.IsInRoleAsync(user, Roles.Partner)) throw new ConflictError("User is already a partner.");
                await userManager.AddToRoleAsync(user, Roles.Partner);
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
                
                await userManager.AddToRoleAsync(user, Roles.Partner);
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
        
        await emailSender.SendEmailAsync(user.Email!, "LugaStore Partner Invitation", $"Click here to accept: {url}");
    }
}
