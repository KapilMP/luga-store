using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.UserManagement.Commands;

public record DeactivateAdminCommand(int UserId) : IRequest;

public class DeactivateAdminCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<DeactivateAdminCommand>
{
    public async Task Handle(DeactivateAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null || !await userManager.IsInRoleAsync(user, Roles.Admin)) throw new NotFoundError("Admin not found.");
        
        user.IsActive = false;
        await userManager.UpdateAsync(user);
    }
}
