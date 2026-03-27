using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Commands;

public record CustomerLoginCommand(string Email, string Password) : IRequest<AuthResult>;
public record AdminLoginCommand(string Email, string Password) : IRequest<AuthResult>;
public record PartnerLoginCommand(string Email, string Password) : IRequest<AuthResult>;
public record PartnerManagerLoginCommand(string Email, string Password) : IRequest<AuthResult>;

public class CustomerLoginCommandHandler(IAuthService authService) : IRequestHandler<CustomerLoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(CustomerLoginCommand request, CancellationToken cancellationToken)
        => authService.CustomerLoginAsync(request.Email, request.Password);
}

public class AdminLoginCommandHandler(IAuthService authService) : IRequestHandler<AdminLoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(AdminLoginCommand request, CancellationToken cancellationToken)
        => authService.AdminLoginAsync(request.Email, request.Password);
}

public class PartnerLoginCommandHandler(IAuthService authService) : IRequestHandler<PartnerLoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(PartnerLoginCommand request, CancellationToken cancellationToken)
        => authService.PartnerLoginAsync(request.Email, request.Password);
}

public class PartnerManagerLoginCommandHandler(IAuthService authService) : IRequestHandler<PartnerManagerLoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(PartnerManagerLoginCommand request, CancellationToken cancellationToken)
        => authService.PartnerManagerLoginAsync(request.Email, request.Password);
}
