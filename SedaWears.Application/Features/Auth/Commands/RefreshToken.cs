using SedaWears.Application.Features.Users;
using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Auth.Models;
using SedaWears.Domain.Entities;
using System.Security.Claims;

namespace SedaWears.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<(AuthResponse Response, string RefreshToken)>;

public class RefreshTokenHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    IOriginContext originContext) : IRequestHandler<RefreshTokenCommand, (AuthResponse Response, string RefreshToken)>
{
    public async Task<(AuthResponse Response, string RefreshToken)> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var role = originContext.CurrentRole;
        var principal = tokenService.GetPrincipalFromExpiredToken(request.RefreshToken) ?? throw new UnauthorizedAccessException("Invalid token");
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var user = await userManager.FindByEmailAsync(email!) ?? throw new UnauthorizedAccessException("User not found");
        if (!user.IsActive || !user.Role.Equals(role)) throw new UnauthorizedAccessException("User not found");

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user);
        return (new AuthResponse(accessToken, user.ToUserRepresentation()), refreshToken);
    }
}
