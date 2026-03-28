using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

// Commands for an Admin to manage Partners and their Managers
public record InvitePartnerCommand(string Email) : IRequest;
public record ResendPartnerInvitationCommand(int PartnerId) : IRequest;
public record DeletePartnerCommand(int PartnerId) : IRequest;
public record ActivatePartnerCommand(int PartnerId) : IRequest;
public record DeactivatePartnerCommand(int PartnerId) : IRequest;

public record InvitePartnerManagerCommand(int PartnerId, string Email) : IRequest;
public record ResendPartnerManagerInvitationCommand(int PartnerId, int ManagerId) : IRequest;
public record ActivatePartnerManagerCommand(int PartnerId, int ManagerId) : IRequest;
public record DeactivatePartnerManagerCommand(int PartnerId, int ManagerId) : IRequest;
public record DeletePartnerManagerCommand(int PartnerId, int ManagerId) : IRequest;

public class PartnerAdminCommandHandlers(IPartnerService partnerService) :
    IRequestHandler<InvitePartnerCommand>,
    IRequestHandler<ResendPartnerInvitationCommand>,
    IRequestHandler<DeletePartnerCommand>,
    IRequestHandler<ActivatePartnerCommand>,
    IRequestHandler<DeactivatePartnerCommand>,
    IRequestHandler<InvitePartnerManagerCommand>,
    IRequestHandler<ResendPartnerManagerInvitationCommand>,
    IRequestHandler<ActivatePartnerManagerCommand>,
    IRequestHandler<DeactivatePartnerManagerCommand>,
    IRequestHandler<DeletePartnerManagerCommand>
{
    public Task Handle(InvitePartnerCommand request, CancellationToken cancellationToken)
        => partnerService.InvitePartnerAsync(request.Email, cancellationToken);

    public Task Handle(ResendPartnerInvitationCommand request, CancellationToken cancellationToken)
        => partnerService.ResendPartnerInvitationAsync(request.PartnerId, cancellationToken);

    public Task Handle(DeletePartnerCommand request, CancellationToken cancellationToken)
        => partnerService.DeletePartnerAsync(request.PartnerId, cancellationToken);

    public Task Handle(ActivatePartnerCommand request, CancellationToken cancellationToken)
        => partnerService.ActivatePartnerAsync(request.PartnerId, cancellationToken);

    public Task Handle(DeactivatePartnerCommand request, CancellationToken cancellationToken)
        => partnerService.DeactivatePartnerAsync(request.PartnerId, cancellationToken);

    public Task Handle(InvitePartnerManagerCommand request, CancellationToken cancellationToken)
        => partnerService.InvitePartnerManagerAsync(request.PartnerId, request.Email, cancellationToken);

    public Task Handle(ResendPartnerManagerInvitationCommand request, CancellationToken cancellationToken)
        => partnerService.ResendPartnerManagerInvitationAsync(request.PartnerId, request.ManagerId, cancellationToken);

    public Task Handle(ActivatePartnerManagerCommand request, CancellationToken cancellationToken)
        => partnerService.ActivatePartnerManagerAsync(request.PartnerId, request.ManagerId, cancellationToken);

    public Task Handle(DeactivatePartnerManagerCommand request, CancellationToken cancellationToken)
        => partnerService.DeactivatePartnerManagerAsync(request.PartnerId, request.ManagerId, cancellationToken);

    public Task Handle(DeletePartnerManagerCommand request, CancellationToken cancellationToken)
        => partnerService.DeletePartnerManagerAsync(request.PartnerId, request.ManagerId, cancellationToken);
}
