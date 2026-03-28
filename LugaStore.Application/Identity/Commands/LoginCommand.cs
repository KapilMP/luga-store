using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;
using LugaStore.Domain.Common;

namespace LugaStore.Application.Identity.Commands;

public abstract record LoginCommand(string Email, string Password);

public record CustomerLoginCommand(string Email, string Password) : LoginCommand(Email, Password), IRequest<AuthResult>;
public record AdminLoginCommand(string Email, string Password) : LoginCommand(Email, Password), IRequest<AuthResult>;
public record PartnerLoginCommand(string Email, string Password) : LoginCommand(Email, Password), IRequest<AuthResult>;
public record PartnerManagerLoginCommand(string Email, string Password) : LoginCommand(Email, Password), IRequest<AuthResult>;

public class CustomerLoginCommandHandler(IAuthService authService) : IRequestHandler<CustomerLoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(CustomerLoginCommand request, CancellationToken cancellationToken)
        => authService.LoginAsync(request.Email, request.Password, Roles.Customer, cancellationToken);
}

public class AdminLoginCommandHandler(IAuthService authService) : IRequestHandler<AdminLoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
        => authService.LoginAsync(request.Email, request.Password, Roles.Admin, cancellationToken);
}

public class PartnerLoginCommandHandler(IAuthService authService) : IRequestHandler<PartnerLoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(PartnerLoginCommand request, CancellationToken cancellationToken)
        => authService.LoginAsync(request.Email, request.Password, Roles.Partner, cancellationToken);
}

public class PartnerManagerLoginCommandHandler(IAuthService authService) : IRequestHandler<PartnerManagerLoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(PartnerManagerLoginCommand request, CancellationToken cancellationToken)
        => authService.LoginAsync(request.Email, request.Password, Roles.PartnerManager, cancellationToken);
}

