using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.UserManagement.Commands;

public record DeactivatePartnerCommand(int PartnerId) : IRequest;

public class DeactivatePartnerCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<DeactivatePartnerCommand>
{
    public async Task Handle(DeactivatePartnerCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.PartnerId.ToString());
        if (user == null || !await userManager.IsInRoleAsync(user, Roles.Partner)) throw new NotFoundError("Partner not found.");
        
        user.IsActive = false;
        await userManager.UpdateAsync(user);
    }
}
