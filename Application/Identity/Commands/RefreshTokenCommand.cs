using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record RefreshTokenCommand(string RefreshToken) : IRequest<(string AccessToken, string RefreshToken)?>;

public class RefreshTokenCommandHandler(IAuthService authService) : IRequestHandler<RefreshTokenCommand, (string AccessToken, string RefreshToken)?>
{
    public async Task<(string AccessToken, string RefreshToken)?> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        => await authService.RefreshTokenAsync(request.RefreshToken);
}
