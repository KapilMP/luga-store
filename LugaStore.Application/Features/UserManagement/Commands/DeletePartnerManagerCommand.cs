using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.UserManagement.Commands;

public record DeletePartnerManagerCommand(int PartnerId, int ManagerId) : IRequest;

public class DeletePartnerManagerCommandHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext) : 
    IRequestHandler<DeletePartnerManagerCommand>
{
    public async Task Handle(DeletePartnerManagerCommand request, CancellationToken cancellationToken)
    {
        var mapping = await dbContext.PartnerManagers
            .FirstOrDefaultAsync(pm => pm.PartnerId == request.PartnerId && pm.ManagerId == request.ManagerId, cancellationToken)
            ?? throw new NotFoundError("Manager assignment not found.");

        var manager = await userManager.FindByIdAsync(request.ManagerId.ToString());
        
        dbContext.PartnerManagers.Remove(mapping);
        await userManager.DeleteAsync(manager!);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
