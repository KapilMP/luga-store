using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.Application.Features.Auth.Commands;

public record CustomerLoginCommand(string Email, string Password) : LoginCommand(Email, Password), IRequest<(AdminAuthResponse<CustomerRepresentation> Response, string RefreshToken)>;

public class CustomerLoginCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService) :
    IRequestHandler<CustomerLoginCommand, (AdminAuthResponse<CustomerRepresentation> Response, string RefreshToken)>
{
    public async Task<(AdminAuthResponse<CustomerRepresentation> Response, string RefreshToken)> Handle(CustomerLoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new NotFoundError("Email or Password is not correct");
        
        if (!user.IsActive) 
            throw new UnauthorizedError("Your account has been deactivated.");
            
        if (!await userManager.IsInRoleAsync(user, Roles.Customer)) 
            throw new NotFoundError("Email or Password is not correct");

        if (user.PasswordHash == null) 
            throw new NotFoundError("Email or Password is not correct");

        if (!await userManager.CheckPasswordAsync(user, request.Password)) 
            throw new NotFoundError("Email or Password is not correct");

        var accessToken = tokenService.GenerateAccessToken(user, Roles.Customer);
        var refreshToken = tokenService.GenerateRefreshToken(user);

        return (new AdminAuthResponse<CustomerRepresentation>(accessToken, user.ToCustomerRepresentation()), refreshToken);
    }
}
