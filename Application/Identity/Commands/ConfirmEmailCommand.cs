using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record ConfirmEmailCommand(string UserId, string Token) : IRequest<bool>;

public class ConfirmEmailCommandHandler(IAuthService authService) : IRequestHandler<ConfirmEmailCommand, bool>
{
    public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        => await authService.ConfirmEmailAsync(request.UserId, request.Token);
}
