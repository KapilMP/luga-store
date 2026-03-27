using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Commands;

public record LoginCommand(string Email, string Password) : IRequest<AuthResult>;

public class CustomerLoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        => authService.CustomerLoginAsync(request.Email, request.Password);
}

public class AdminLoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        => authService.AdminLoginAsync(request.Email, request.Password);
}

public class PartnerLoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        => authService.PartnerLoginAsync(request.Email, request.Password);
}

public class PartnerManagerLoginCommandHandler(IAuthService authService) : IRequestHandler<LoginCommand, AuthResult>
{
    public Task<AuthResult> Handle(LoginCommand request, CancellationToken cancellationToken)
        => authService.PartnerManagerLoginAsync(request.Email, request.Password);
}
