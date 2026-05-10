using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Users.Commands;

public record UpdateUserActiveStatusCommand(int UserId, bool IsActive, UserRole Role) : IRequest;

public class UpdateUserActiveStatusValidator : AbstractValidator<UpdateUserActiveStatusCommand>
{
    public UpdateUserActiveStatusValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}

public class UpdateUserActiveStatusHandler(UserManager<User> userManager) : IRequestHandler<UpdateUserActiveStatusCommand>
{
    public async Task Handle(UpdateUserActiveStatusCommand request, CancellationToken ct)
    {
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Id == request.UserId && u.Role == request.Role, ct)
            ?? throw new NotFoundException("User not found.");

        user.IsActive = request.IsActive;
        await userManager.UpdateAsync(user);
    }
}
