using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Auth.Commands;

public record RegisterCommand(string Email, string Password, string FirstName, string LastName, string Phone) : IRequest<AuthResponse>;

public class RegisterValidator : AbstractValidator<RegisterCommand>
{
    public RegisterValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Phone).NotEmpty();
    }
}

public class RegisterHandler(
    UserManager<User> userManager,
    ITokenService tokenService) : IRequestHandler<RegisterCommand, AuthResponse>
{
    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken ct)
    {
        var user = new User { Email = request.Email, UserName = request.Email, FirstName = request.FirstName, LastName = request.LastName, PhoneNumber = request.Phone };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new BadRequestError(result.Errors.First().Description);
        
        await userManager.AddToRoleAsync(user, Roles.Customer);
        
        var accessToken = tokenService.GenerateAccessToken(user, Roles.Customer);
        var refreshToken = tokenService.GenerateRefreshToken(user);
        return new AuthResponse(accessToken, refreshToken, UserRepresentation.ToUserRepresentation(user));
    }
}
