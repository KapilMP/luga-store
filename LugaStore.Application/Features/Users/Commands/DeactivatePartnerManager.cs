using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Users.Commands;

public record DeactivatePartnerManagerCommand(int ManagerId) : IRequest;

public class DeactivatePartnerManagerValidator : AbstractValidator<DeactivatePartnerManagerCommand>
{
    public DeactivatePartnerManagerValidator()
    {
        RuleFor(x => x.ManagerId).GreaterThan(0);
    }
}

public class DeactivatePartnerManagerHandler(UserManager<User> userManager, IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<DeactivatePartnerManagerCommand>
{
    public async Task Handle(DeactivatePartnerManagerCommand request, CancellationToken ct)
    {
        var partnerId = currentUser.Id!.Value;
        var pm = await dbContext.PartnerManagers
            .FirstOrDefaultAsync(x => x.PartnerId == partnerId && x.ManagerId == request.ManagerId, ct)
            ?? throw new NotFoundError("Partner Manager linkage not found");

        var user = await userManager.FindByIdAsync(request.ManagerId.ToString()) ?? throw new NotFoundError("User not found");
        user.IsActive = false;
        await userManager.UpdateAsync(user);
    }
}
