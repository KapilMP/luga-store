using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record ActivateAdminCommand(int UserId) : IRequest;
public record DeactivateAdminCommand(int UserId) : IRequest;
public record ActivatePartnerCommand(int UserId) : IRequest;
public record DeactivatePartnerCommand(int UserId) : IRequest;
public record ActivatePartnerManagerCommand(int UserId) : IRequest;
public record DeactivatePartnerManagerCommand(int UserId) : IRequest;

public class ActivateAdminCommandHandler(IUserService userService) : IRequestHandler<ActivateAdminCommand>
{
    public Task Handle(ActivateAdminCommand request, CancellationToken cancellationToken)
        => userService.ActivateAdminAsync(request.UserId, cancellationToken);
}

public class DeactivateAdminCommandHandler(IUserService userService) : IRequestHandler<DeactivateAdminCommand>
{
    public Task Handle(DeactivateAdminCommand request, CancellationToken cancellationToken)
        => userService.DeactivateAdminAsync(request.UserId, cancellationToken);
}

public class ActivatePartnerCommandHandler(IUserService userService) : IRequestHandler<ActivatePartnerCommand>
{
    public Task Handle(ActivatePartnerCommand request, CancellationToken cancellationToken)
        => userService.ActivatePartnerAsync(request.UserId, cancellationToken);
}

public class DeactivatePartnerCommandHandler(IUserService userService) : IRequestHandler<DeactivatePartnerCommand>
{
    public Task Handle(DeactivatePartnerCommand request, CancellationToken cancellationToken)
        => userService.DeactivatePartnerAsync(request.UserId, cancellationToken);
}

public class ActivatePartnerManagerCommandHandler(IUserService userService) : IRequestHandler<ActivatePartnerManagerCommand>
{
    public Task Handle(ActivatePartnerManagerCommand request, CancellationToken cancellationToken)
        => userService.ActivatePartnerManagerAsync(request.UserId, cancellationToken);
}

public class DeactivatePartnerManagerCommandHandler(IUserService userService) : IRequestHandler<DeactivatePartnerManagerCommand>
{
    public Task Handle(DeactivatePartnerManagerCommand request, CancellationToken cancellationToken)
        => userService.DeactivatePartnerManagerAsync(request.UserId, cancellationToken);
}
