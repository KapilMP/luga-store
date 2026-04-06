using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.UserManagement.Commands;

public record DeactivatePartnerManagerCommand(int PartnerId, int ManagerId) : IRequest;

public class DeactivatePartnerManagerCommandHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext) : 
    IRequestHandler<DeactivatePartnerManagerCommand>
{
    public async Task Handle(DeactivatePartnerManagerCommand request, CancellationToken cancellationToken)
    {
        var mapping = await dbContext.PartnerManagers
            .FirstOrDefaultAsync(pm => pm.PartnerId == request.PartnerId && pm.ManagerId == request.ManagerId, cancellationToken)
            ?? throw new NotFoundError("Manager assignment not found.");

        var manager = await userManager.FindByIdAsync(request.ManagerId.ToString());
        manager!.IsActive = false;
        await userManager.UpdateAsync(manager);
    }
}
