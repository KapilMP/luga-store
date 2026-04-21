using SedaWears.Application.Features.Auth.Models;
using SedaWears.Application.Features.Users;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;

namespace SedaWears.Application.Features.Auth.Commands;

public record AcceptInvitationCommand(
    string Email,
    string Token,
    string FirstName,
    string LastName,
    string Password,
    string Role) : IRequest<(AuthResponse Response, string RefreshToken)>;

public class AcceptInvitationValidator : AbstractValidator<AcceptInvitationCommand>
{
    public AcceptInvitationValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Token).NotEmpty();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.Password).NotEmpty().MinimumLength(6);
        RuleFor(x => x.Role).NotEmpty();
    }
}

public class AcceptInvitationHandler(
    UserManager<User> userManager,
    ITokenService tokenService) : IRequestHandler<AcceptInvitationCommand, (AuthResponse Response, string RefreshToken)>
{
    public async Task<(AuthResponse Response, string RefreshToken)> Handle(AcceptInvitationCommand request, CancellationToken ct)
    {
        if (!Enum.TryParse<UserRole>(request.Role, true, out var roleEnum))
            throw new BadRequestException("Invalid role provided.");

        var user = await userManager.Users
            .FirstOrDefaultAsync(u => u.Email == request.Email && u.Role == roleEnum, ct)
            ?? throw new NotFoundException("User not found.");

        if (user.EmailConfirmed)
            throw new BadRequestException("Invitation already accepted.");

        var confirmationResult = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!confirmationResult.Succeeded)
            throw new BadRequestException("Invalid or expired invitation token.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;

        var updateResult = await userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
            throw new BadRequestException(updateResult.Errors.First().Description);

        var passwordResult = await userManager.AddPasswordAsync(user, request.Password);
        if (!passwordResult.Succeeded)
            throw new BadRequestException(passwordResult.Errors.First().Description);

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user);

        return (new AuthResponse(accessToken, user.ToUserRepresentation()), refreshToken);
    }
}
