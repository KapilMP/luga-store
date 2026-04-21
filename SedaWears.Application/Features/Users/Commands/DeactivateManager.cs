using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Users.Commands;

public record DeactivateManagerCommand(int ManagerId) : IRequest;

public class DeactivateManagerValidator : AbstractValidator<DeactivateManagerCommand>
{
    public DeactivateManagerValidator()
    {
        RuleFor(x => x.ManagerId).GreaterThan(0);
    }
}

public class DeactivateManagerHandler(UserManager<User> userManager, IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<DeactivateManagerCommand>
{
    public async Task Handle(DeactivateManagerCommand request, CancellationToken ct)
    {
        var shopId = currentUser.ShopId ?? throw new UnauthorizedAccessException("Shop context missing. Use X-Shop-ID.");
        
        var sm = await dbContext.ShopManagers
            .FirstOrDefaultAsync(x => x.ShopId == shopId && x.ManagerId == request.ManagerId, ct)
            ?? throw new NotFoundException("Shop Manager linkage not found");

        var user = await userManager.FindByIdAsync(request.ManagerId.ToString()) ?? throw new NotFoundException("User not found");
        user.IsActive = false;
        await userManager.UpdateAsync(user);
    }
}
