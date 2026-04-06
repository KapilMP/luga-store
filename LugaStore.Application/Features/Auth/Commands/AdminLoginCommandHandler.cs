using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.Application.Features.Auth.Commands;

public class AdminLoginCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService) :
    ICommandHandler<AdminLoginCommand, (AdminAuthResponse<AdminRepresentation> Response, string RefreshToken)>
{
    public async Task<(AdminAuthResponse<AdminRepresentation> Response, string RefreshToken)> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new NotFoundError("Email or Password is not correct");

        if (!user.IsActive)
            throw new UnauthorizedError("Your account has been deactivated.");

        if (!await userManager.IsInRoleAsync(user, Roles.Admin))
            throw new NotFoundError("Email or Password is not correct");

        if (!user.EmailConfirmed)
            throw new NotFoundError("Email or Password is not correct");

        if (!await userManager.CheckPasswordAsync(user, request.Password))
            throw new NotFoundError("Email or Password is not correct");

        var accessToken = tokenService.GenerateAccessToken(user, Roles.Admin);
        var refreshToken = tokenService.GenerateRefreshToken(user);

        return (new AdminAuthResponse<AdminRepresentation>(accessToken, user.ToAdminRepresentation()), refreshToken);
    }
}
