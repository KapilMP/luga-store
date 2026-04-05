using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.Application.UserManagement.Commands;

public record DeleteAdminCommand(int UserId) : IRequest;

public class DeleteAdminCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<DeleteAdminCommand>
{
    public async Task Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Admin not found.");
        
        if (!await userManager.IsInRoleAsync(user, Roles.Admin))
        {
             throw new BadRequestError("User is not an admin.");
        }

        // Check if user has other roles (e.g. Partner, Customer)
        var roles = await userManager.GetRolesAsync(user);
        
        if (roles.Count == 1)
        {
            // If they only have the Admin role, delete the user entirely
            await userManager.DeleteAsync(user);
        }
        else
        {
            // Otherwise, just remove the Admin role
            await userManager.RemoveFromRoleAsync(user, Roles.Admin);
        }
    }
}
