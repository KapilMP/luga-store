using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Users.Commands;

public record ActivatePartnerManagerCommand(int PartnerId, int ManagerId) : IRequest;

public class ActivatePartnerManagerValidator : AbstractValidator<ActivatePartnerManagerCommand>
{
    public ActivatePartnerManagerValidator()
    {
        RuleFor(x => x.PartnerId).GreaterThan(0);
        RuleFor(x => x.ManagerId).GreaterThan(0);
    }
}

public class ActivatePartnerManagerHandler(UserManager<User> userManager, IApplicationDbContext dbContext) : IRequestHandler<ActivatePartnerManagerCommand>
{
    public async Task Handle(ActivatePartnerManagerCommand request, CancellationToken ct)
    {
        var pm = await dbContext.PartnerManagers
            .FirstOrDefaultAsync(x => x.PartnerId == request.PartnerId && x.ManagerId == request.ManagerId, ct)
            ?? throw new NotFoundError("Partner Manager linkage not found");

        var user = await userManager.FindByIdAsync(request.ManagerId.ToString()) ?? throw new NotFoundError("User not found");
        user.IsActive = true;
        await userManager.UpdateAsync(user);
    }
}
