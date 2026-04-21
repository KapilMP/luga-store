using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Users.Commands;

public record ActivateManagerCommand(int ManagerId) : IRequest;

public class ActivateManagerValidator : AbstractValidator<ActivateManagerCommand>
{
    public ActivateManagerValidator()
    {
        RuleFor(x => x.ManagerId)
            .GreaterThan(0).WithMessage("A valid manager identifier is required.");
    }
}

public class ActivateManagerHandler(UserManager<User> userManager, IApplicationDbContext dbContext, ICurrentUser currentUser) : IRequestHandler<ActivateManagerCommand>
{
    public async Task Handle(ActivateManagerCommand request, CancellationToken ct)
    {
        var shopId = currentUser.ShopId ?? throw new UnauthorizedAccessException("Shop context missing. Use X-Shop-ID.");
        
        var sm = await dbContext.ShopManagers
            .FirstOrDefaultAsync(x => x.ShopId == shopId && x.ManagerId == request.ManagerId, ct)
            ?? throw new NotFoundException("Shop Manager linkage not found");

        var user = await userManager.FindByIdAsync(request.ManagerId.ToString()) ?? throw new NotFoundException("User not found");
        user.IsActive = true;
        await userManager.UpdateAsync(user);
    }
}
