using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Domain.Entities;
using System.Security.Claims;

namespace LugaStore.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken, string Role) : IRequest<(AuthResponse Response, string RefreshToken)>;

public class RefreshTokenHandler(
    UserManager<User> userManager,
    ITokenService tokenService) : IRequestHandler<RefreshTokenCommand, (AuthResponse Response, string RefreshToken)>
{
    public async Task<(AuthResponse Response, string RefreshToken)> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(request.RefreshToken) ?? throw new UnauthorizedError("Invalid token");
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var user = await userManager.FindByEmailAsync(email!) ?? throw new UnauthorizedError("User not found");
        if (!user.IsActive || !await userManager.IsInRoleAsync(user, request.Role)) throw new UnauthorizedError("Access denied");

        var accessToken = tokenService.GenerateAccessToken(user, request.Role);
        var refreshToken = tokenService.GenerateRefreshToken(user);
        return (new AuthResponse(accessToken, UserRepresentation.ToUserRepresentation(user)), refreshToken);
    }
}
