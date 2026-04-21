using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Application.Features.Users.Models;

namespace SedaWears.Application.Features.Users.Commands;

public record UpdateUserCommand(int Id, string FirstName, string LastName, bool? IsActive, string? NewPassword = null) : IRequest<BaseUserRepresentation>;

public class UpdateUserValidator : AbstractValidator<UpdateUserCommand>
{
    public UpdateUserValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.");
            
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.");
    }
}

public class UpdateUserHandler(UserManager<User> userManager) : IRequestHandler<UpdateUserCommand, BaseUserRepresentation>
{
    public async Task<BaseUserRepresentation> Handle(UpdateUserCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString()) ?? throw new NotFoundException("User not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        if (request.IsActive.HasValue) user.IsActive = request.IsActive.Value;

        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);

        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            await userManager.ResetPasswordAsync(user, token, request.NewPassword);
        }

        return user.ToUserRepresentation();
    }
}
