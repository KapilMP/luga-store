using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

// Commands for a Partner to manage their own Managers
public record InviteManagerCommand(string Email) : IRequest;
public record ResendManagerInvitationCommand(int ManagerId) : IRequest;
public record ActivateManagerCommand(int ManagerId) : IRequest;
public record DeactivateManagerCommand(int ManagerId) : IRequest;
public record DeleteManagerCommand(int ManagerId) : IRequest;

public class PartnerContextCommandHandlers(IPartnerService partnerService) :
    IRequestHandler<InviteManagerCommand>,
    IRequestHandler<ResendManagerInvitationCommand>,
    IRequestHandler<ActivateManagerCommand>,
    IRequestHandler<DeactivateManagerCommand>,
    IRequestHandler<DeleteManagerCommand>
{
    public Task Handle(InviteManagerCommand request, CancellationToken cancellationToken)
        => partnerService.InviteManagerAsync(request.Email, cancellationToken);

    public Task Handle(ResendManagerInvitationCommand request, CancellationToken cancellationToken)
        => partnerService.ResendManagerInvitationAsync(request.ManagerId, cancellationToken);

    public Task Handle(ActivateManagerCommand request, CancellationToken cancellationToken)
        => partnerService.ActivateManagerAsync(request.ManagerId, cancellationToken);

    public Task Handle(DeactivateManagerCommand request, CancellationToken cancellationToken)
        => partnerService.DeactivateManagerAsync(request.ManagerId, cancellationToken);

    public Task Handle(DeleteManagerCommand request, CancellationToken cancellationToken)
        => partnerService.DeleteManagerAsync(request.ManagerId, cancellationToken);
}
