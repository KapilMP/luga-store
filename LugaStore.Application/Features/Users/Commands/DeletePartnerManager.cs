using MediatR;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;

namespace LugaStore.Application.Features.Users.Commands;

public record DeletePartnerManagerCommand(int ManagerId) : IRequest;

public class DeletePartnerManagerValidator : AbstractValidator<DeletePartnerManagerCommand>
{
    public DeletePartnerManagerValidator()
    {
        RuleFor(x => x.ManagerId).GreaterThan(0);
    }
}

public class DeletePartnerManagerHandler(IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<DeletePartnerManagerCommand>
{
    public async Task Handle(DeletePartnerManagerCommand request, CancellationToken ct)
    {
        var partnerId = currentUser.Id!.Value;
        var pm = await dbContext.PartnerManagers
            .FirstOrDefaultAsync(pm => pm.PartnerId == partnerId && pm.ManagerId == request.ManagerId, ct)
            ?? throw new NotFoundError("Partner Manager linkage not found");

        dbContext.PartnerManagers.Remove(pm);
        await dbContext.SaveChangesAsync(ct);
    }
}
