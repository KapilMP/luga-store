using System.Security.Claims;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Features.Auth.Models;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Domain.Entities;
using LugaStore.Domain.Common;

namespace LugaStore.Application.Features.Auth.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<(string AccessToken, string RefreshToken, object User)?>;
public record CustomerRefreshTokenCommand(string RefreshToken) : IRequest<(string AccessToken, string RefreshToken, CustomerRepresentation User)?>;
public record AdminRefreshTokenCommand(string RefreshToken) : IRequest<(string AccessToken, string RefreshToken, AdminRepresentation User)?>;
public record PartnerRefreshTokenCommand(string RefreshToken) : IRequest<(string AccessToken, string RefreshToken, PartnerRepresentation User)?>;
public record PartnerManagerRefreshTokenCommand(string RefreshToken) : IRequest<(string AccessToken, string RefreshToken, PartnerManagerRepresentation User)?>;

public class RefreshTokenCommandHandler(
    UserManager<User> userManager,
    ITokenService tokenService) :
    IRequestHandler<RefreshTokenCommand, (string AccessToken, string RefreshToken, object User)?>,
    IRequestHandler<CustomerRefreshTokenCommand, (string AccessToken, string RefreshToken, CustomerRepresentation User)?>,
    IRequestHandler<AdminRefreshTokenCommand, (string AccessToken, string RefreshToken, AdminRepresentation User)?>,
    IRequestHandler<PartnerRefreshTokenCommand, (string AccessToken, string RefreshToken, PartnerRepresentation User)?>,
    IRequestHandler<PartnerManagerRefreshTokenCommand, (string AccessToken, string RefreshToken, PartnerManagerRepresentation User)?>
{
    public async Task<(string AccessToken, string RefreshToken, object User)?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(request.RefreshToken);
        if (principal == null) return null;

        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return null;

        var user = await userManager.FindByEmailAsync(email);
        if (user == null || !user.IsActive) return null;

        var roles = principal.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        if (roles.Count == 0) return null;

        var role = roles[0];
        return await RefreshAsync<object>(request.RefreshToken, role, cancellationToken);
    }

    public async Task<(string AccessToken, string RefreshToken, CustomerRepresentation User)?> Handle(CustomerRefreshTokenCommand request, CancellationToken cancellationToken)
        => await RefreshAsync<CustomerRepresentation>(request.RefreshToken, Roles.Customer, cancellationToken);

    public async Task<(string AccessToken, string RefreshToken, AdminRepresentation User)?> Handle(AdminRefreshTokenCommand request, CancellationToken cancellationToken)
        => await RefreshAsync<AdminRepresentation>(request.RefreshToken, Roles.Admin, cancellationToken);

    public async Task<(string AccessToken, string RefreshToken, PartnerRepresentation User)?> Handle(PartnerRefreshTokenCommand request, CancellationToken cancellationToken)
        => await RefreshAsync<PartnerRepresentation>(request.RefreshToken, Roles.Partner, cancellationToken);

    public async Task<(string AccessToken, string RefreshToken, PartnerManagerRepresentation User)?> Handle(PartnerManagerRefreshTokenCommand request, CancellationToken cancellationToken)
        => await RefreshAsync<PartnerManagerRepresentation>(request.RefreshToken, Roles.PartnerManager, cancellationToken);

    private async Task<(string AccessToken, string RefreshToken, T User)?> RefreshAsync<T>(string refreshToken, string role, CancellationToken cancellationToken) where T : class
    {
        var principal = tokenService.GetPrincipalFromExpiredToken(refreshToken);
        if (principal == null) return null;

        var email = principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return null;

        var user = await userManager.FindByEmailAsync(email);
        if (user == null || !user.IsActive) return null;
        
        if (!await userManager.IsInRoleAsync(user, role)) return null;

        var newAccessToken = tokenService.GenerateAccessToken(user, role);
        var newRefreshToken = tokenService.GenerateRefreshToken(user);

        object representation = role switch
        {
            Roles.Admin => user.ToAdminRepresentation(),
            Roles.Partner => user.ToPartnerRepresentation(),
            Roles.PartnerManager => user.ToPartnerManagerRepresentation(),
            _ => user.ToCustomerRepresentation()
        };

        return (newAccessToken, newRefreshToken, (T)representation);
    }
}

public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommand>
{
    public RefreshTokenCommandValidator()
    {
        RuleFor(v => v.RefreshToken).NotEmpty();
    }
}
