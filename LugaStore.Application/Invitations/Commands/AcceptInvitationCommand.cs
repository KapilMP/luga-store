using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Invitations.Commands;

public record AcceptInvitationCommand(string Email, string Token, string Password, string FirstName, string LastName) : IRequest<bool>;

public class AcceptInvitationCommandHandler(UserManager<User> userManager) :
    IRequestHandler<AcceptInvitationCommand, bool>
{
    public async Task<bool> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null || user.EmailConfirmed) return false;

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        await userManager.UpdateAsync(user);

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded) return false;

        var addPassword = await userManager.AddPasswordAsync(user, request.Password);
        return addPassword.Succeeded;
    }
}
