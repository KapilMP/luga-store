using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record ResendInvitationCommand(string Email) : IRequest<bool>;

public class ResendInvitationCommandHandler(IAuthService authService) : IRequestHandler<ResendInvitationCommand, bool>
{
    public async Task<bool> Handle(ResendInvitationCommand request, CancellationToken cancellationToken)
        => await authService.ResendInvitationAsync(request.Email);
}
