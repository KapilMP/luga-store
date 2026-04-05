using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Auth.Commands;

public record ChangePasswordCommand(string UserId, string CurrentPassword, string NewPassword) : IRequest<bool>;

public class ChangePasswordCommandHandler(UserManager<User> userManager) :
    IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null) return false;
        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        return result.Succeeded;
    }
}
