using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Auth.Models;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.Application.Auth.Commands;

public record GoogleLoginCommand(string IdToken) : IRequest<(AdminAuthResponse<CustomerRepresentation> Response, string RefreshToken)?>;

public class GoogleLoginCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService,
    IGoogleAuthService googleAuthService) :
    IRequestHandler<GoogleLoginCommand, (AdminAuthResponse<CustomerRepresentation> Response, string RefreshToken)?>
{
    public async Task<(AdminAuthResponse<CustomerRepresentation> Response, string RefreshToken)?> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
    {
        var payload = await googleAuthService.ValidateTokenAsync(request.IdToken);
        if (payload == null) return null;

        return await LoginExternalAsync(payload.Email, payload.GivenName, payload.FamilyName, cancellationToken);
    }

    private async Task<(AdminAuthResponse<CustomerRepresentation> Response, string RefreshToken)?> LoginExternalAsync(string email, string firstName, string lastName, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User
            {
                Email = email,
                UserName = Guid.NewGuid().ToString(),
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true
            };
            var result = await userManager.CreateAsync(user);
            if (!result.Succeeded) return null;
            await userManager.AddToRoleAsync(user, Roles.Customer);
        }

        var roles = await userManager.GetRolesAsync(user);
        var accessToken = tokenService.GenerateAccessToken(user, roles.FirstOrDefault() ?? Roles.Customer);
        var refreshToken = tokenService.GenerateRefreshToken(user);

        return (new AdminAuthResponse<CustomerRepresentation>(accessToken, user.ToCustomerRepresentation()), refreshToken);
    }
}

public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginCommandValidator()
    {
        RuleFor(v => v.IdToken).NotEmpty().WithMessage("Google ID Token is required.");
    }
}
