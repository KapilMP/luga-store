using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.Application.Features.Auth.Commands;

public class PartnerManagerLoginCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService) :
    ICommandHandler<PartnerManagerLoginCommand, (AdminAuthResponse<PartnerManagerRepresentation> Response, string RefreshToken)>
{
    public async Task<(AdminAuthResponse<PartnerManagerRepresentation> Response, string RefreshToken)> Handle(PartnerManagerLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new NotFoundError("Email or Password is not correct");
        
        if (!user.IsActive) 
            throw new UnauthorizedError("Your account has been deactivated.");
            
        if (!await userManager.IsInRoleAsync(user, Roles.PartnerManager)) 
            throw new NotFoundError("Email or Password is not correct");

        if (!user.EmailConfirmed) 
            throw new NotFoundError("Email or Password is not correct");

        if (!await userManager.CheckPasswordAsync(user, request.Password)) 
            throw new NotFoundError("Email or Password is not correct");

        var accessToken = tokenService.GenerateAccessToken(user, Roles.PartnerManager);
        var refreshToken = tokenService.GenerateRefreshToken(user);

        return (new AdminAuthResponse<PartnerManagerRepresentation>(accessToken, user.ToPartnerManagerRepresentation()), refreshToken);
    }
}
