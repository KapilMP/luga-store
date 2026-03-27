using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Commands;

public record GoogleLoginCommand(string IdToken) : IRequest<AuthResult?>;

public class GoogleLoginCommandHandler(IAuthService authService) : IRequestHandler<GoogleLoginCommand, AuthResult?>
{
    public async Task<AuthResult?> Handle(GoogleLoginCommand request, CancellationToken cancellationToken)
        => await authService.LoginWithGoogleAsync(request.IdToken, cancellationToken);
}
