using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.UserManagement.Commands;

public record DeletePartnerCommand(int PartnerId) : IRequest;

public class DeletePartnerCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<DeletePartnerCommand>
{
    public async Task Handle(DeletePartnerCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.PartnerId.ToString());
        if (user == null || !await userManager.IsInRoleAsync(user, Roles.Partner)) throw new NotFoundError("Partner not found.");
        
        await userManager.DeleteAsync(user);
    }
}
