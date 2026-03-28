using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

// Commands for an Admin to manage other Admins
public record InviteAdminCommand(string Email) : IRequest;
public record ResendAdminInvitationCommand(int UserId) : IRequest;
public record DeleteAdminCommand(int UserId) : IRequest;
public record ActivateAdminCommand(int UserId) : IRequest;
public record DeactivateAdminCommand(int UserId) : IRequest;

public class AdminIdentityCommandHandlers(IUserService userService) : 
    IRequestHandler<InviteAdminCommand>,
    IRequestHandler<ResendAdminInvitationCommand>,
    IRequestHandler<DeleteAdminCommand>,
    IRequestHandler<ActivateAdminCommand>,
    IRequestHandler<DeactivateAdminCommand>
{
    public Task Handle(InviteAdminCommand request, CancellationToken cancellationToken)
        => userService.InviteAdminAsync(request.Email, cancellationToken);

    public Task Handle(ResendAdminInvitationCommand request, CancellationToken cancellationToken)
        => userService.ResendAdminInvitationAsync(request.UserId, cancellationToken);

    public Task Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
        => userService.DeleteAdminAsync(request.UserId, cancellationToken);

    public Task Handle(ActivateAdminCommand request, CancellationToken cancellationToken)
        => userService.ActivateAdminAsync(request.UserId, cancellationToken);

    public Task Handle(DeactivateAdminCommand request, CancellationToken cancellationToken)
        => userService.DeactivateAdminAsync(request.UserId, cancellationToken);
}
