using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record ResendInvitationCommand(string Email) : IRequest;

public class ResendInvitationCommandHandler(IUserService userService) : IRequestHandler<ResendInvitationCommand>
{
    public async Task Handle(ResendInvitationCommand request, CancellationToken cancellationToken)
        => await userService.ResendInvitationAsync(request.Email);
}
