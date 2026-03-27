using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record AcceptInvitationCommand(string Email, string Token, string Password) : IRequest<bool>;

public class AcceptInvitationCommandHandler(IAuthService authService) : IRequestHandler<AcceptInvitationCommand, bool>
{
    public async Task<bool> Handle(AcceptInvitationCommand request, CancellationToken cancellationToken)
        => await authService.AcceptInvitationAsync(request.Email, request.Token, request.Password, cancellationToken);
}
