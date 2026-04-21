using SedaWears.Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Auth.Models;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Auth.Commands;

public record RegisterCommand(string Email, string Password, string FirstName, string LastName, string Phone) : IRequest<(AuthResponse Response, string RefreshToken)>;

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
    ITokenService tokenService) : IRequestHandler<RegisterCommand, (AuthResponse Response, string RefreshToken)>
{
    public async Task<(AuthResponse Response, string RefreshToken)> Handle(RegisterCommand request, CancellationToken ct)
    {
        var user = new User
        {
            Email = request.Email,
            UserName = Guid.NewGuid().ToString(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            PhoneNumber = request.Phone,
            Role = UserRole.Customer
        };
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user);
        return (new AuthResponse(accessToken, user.ToUserRepresentation()), refreshToken);
    }
}
