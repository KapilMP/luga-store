using SedaWears.Application.Features.Users;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Auth.Models;
using SedaWears.Domain.Entities;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<(AuthResponse Response, string RefreshToken)>;

public class RefreshTokenHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    ITokenService tokenService,
    IOriginContext originContext) : IRequestHandler<RefreshTokenCommand, (AuthResponse Response, string RefreshToken)>
{
    public async Task<(AuthResponse Response, string RefreshToken)> Handle(RefreshTokenCommand request, CancellationToken ct)
    {
        var role = originContext.CurrentRole;
        var principal = tokenService.GetPrincipalFromExpiredToken(request.RefreshToken) ?? throw new UnauthorizedAccessException("Invalid token");
        var email = principal.FindFirstValue(ClaimTypes.Email);
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Email == email && u.Role == role && u.IsActive, ct)
            ?? throw new UnauthorizedAccessException("User not found");

        var accessToken = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken(user);
        return (new AuthResponse(accessToken, user.ToUserRepresentation()), refreshToken);
    }
}
