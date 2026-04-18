using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Auth.Commands;

public record LoginCommand(string Email, string Password, string Role) : IRequest<(AuthResponse Response, string RefreshToken)>;

public class LoginValidator : AbstractValidator<LoginCommand>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
        RuleFor(x => x.Role).NotEmpty();
    }
}

public class LoginHandler(
    UserManager<User> userManager,
    ITokenService tokenService) : IRequestHandler<LoginCommand, (AuthResponse Response, string RefreshToken)>
{
    public async Task<(AuthResponse Response, string RefreshToken)> Handle(LoginCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new NotFoundError("Invalid credentials");
        if (!user.IsActive) throw new UnauthorizedAccessException("Account deactivated");
        if (!await userManager.IsInRoleAsync(user, request.Role)) throw new ForbiddenError("Access denied");
        if (!await userManager.CheckPasswordAsync(user, request.Password)) throw new NotFoundError("Invalid credentials");

        var accessToken = tokenService.GenerateAccessToken(user, request.Role);
        var refreshToken = tokenService.GenerateRefreshToken(user);
        return (new AuthResponse(accessToken, UserRepresentation.ToUserRepresentation(user)), refreshToken);
    }
}
