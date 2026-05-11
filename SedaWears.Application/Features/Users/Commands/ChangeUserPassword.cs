using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Validators;

using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Users.Commands;

public record ChangeUserPasswordCommand(string NewPassword) : IRequest;

public class ChangeUserPasswordValidator : AbstractValidator<ChangeUserPasswordCommand>
{
    public ChangeUserPasswordValidator()
    {
        RuleFor(x => x.NewPassword)
            .Password();
    }
}

public class ChangeUserPasswordHandler(
    UserManager<User> userManager,
    IOriginContext originContext,
    ICurrentUser currentUser) : IRequestHandler<ChangeUserPasswordCommand>
{
    public async Task Handle(ChangeUserPasswordCommand request, CancellationToken ct)
    {
        var userId = currentUser.Id!.Value;
        var role = originContext.CurrentRole;

        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Id == userId && u.Role == role && u.IsActive, ct)
            ?? throw new NotFoundException($"User not found.");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);
        var result = await userManager.ResetPasswordAsync(user, token, request.NewPassword);

        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);
    }
}
