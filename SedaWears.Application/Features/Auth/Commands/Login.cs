using MediatR;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password, bool RememberMe = false) : IRequest<(int Id, UserRole Role)>;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Please enter a valid email address.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.");
    }
}

public class LoginHandler(
    UserManager<User> userManager,
    IOriginContext originContext) : IRequestHandler<LoginCommand, (int Id, UserRole Role)>
{
    public async Task<(int Id, UserRole Role)> Handle(LoginCommand request, CancellationToken ct)
    {
        var role = originContext.CurrentRole;
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == role && u.IsActive, ct)
            ?? throw new UnauthorizedAccessException("Incorrect email or password.");

        if (!await userManager.CheckPasswordAsync(user, request.Password)) throw new UnauthorizedAccessException("Incorrect email or password");

        return (user.Id, user.Role);
    }
}
