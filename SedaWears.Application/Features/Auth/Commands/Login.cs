using SedaWears.Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Auth.Models;
using SedaWears.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password) : IRequest<(AuthResponse Response, string RefreshToken)>;

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
    ITokenService tokenService,
    IOriginContext originContext) : IRequestHandler<LoginCommand, (AuthResponse Response, string RefreshToken)>
{
    public async Task<(AuthResponse Response, string RefreshToken)> Handle(LoginCommand request, CancellationToken ct)
    {
        var role = originContext.CurrentRole;
        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == role && u.IsActive, ct)
            ?? throw new UnauthorizedAccessException("Incorrect email or password");
        
        if (!await userManager.CheckPasswordAsync(user, request.Password)) throw new UnauthorizedAccessException("Incorrect email or password");

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user);
        return (new AuthResponse(accessToken, user.ToUserRepresentation()), refreshToken);
    }
}
