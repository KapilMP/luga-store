using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.UserManagement.Commands;

public record ActivateAdminCommand(int UserId) : IRequest;

public class ActivateAdminCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<ActivateAdminCommand>
{
    public async Task Handle(ActivateAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null || !await userManager.IsInRoleAsync(user, Roles.Admin)) throw new NotFoundError("Admin not found.");
        
        user.IsActive = true;
        await userManager.UpdateAsync(user);
    }
}
