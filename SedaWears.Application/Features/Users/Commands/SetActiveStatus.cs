using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Users.Commands;

public record SetUserActiveStatusCommand(int UserId, bool IsActive) : IRequest;

public class SetUserActiveStatusValidator : AbstractValidator<SetUserActiveStatusCommand>
{
    public SetUserActiveStatusValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
    }
}

public class SetUserActiveStatusHandler(UserManager<User> userManager) : IRequestHandler<SetUserActiveStatusCommand>
{
    public async Task Handle(SetUserActiveStatusCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundException("User not found.");
        user.IsActive = request.IsActive;
        await userManager.UpdateAsync(user);
    }
}
