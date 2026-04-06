using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.UserManagement.Commands;

public record ActivatePartnerCommand(int PartnerId) : IRequest;

public class ActivatePartnerCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<ActivatePartnerCommand>
{
    public async Task Handle(ActivatePartnerCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.PartnerId.ToString());
        if (user == null || !await userManager.IsInRoleAsync(user, Roles.Partner)) throw new NotFoundError("Partner not found.");
        
        user.IsActive = true;
        await userManager.UpdateAsync(user);
    }
}
