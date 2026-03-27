using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record DeleteAdminCommand(int UserId) : IRequest;
public record DeletePartnerCommand(int UserId) : IRequest;
public record DeletePartnerManagerCommand(int UserId) : IRequest;

public class DeleteAdminCommandHandler(IUserService userService) : IRequestHandler<DeleteAdminCommand>
{
    public Task Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
        => userService.DeleteAdminAsync(request.UserId, cancellationToken);
}

public class DeletePartnerCommandHandler(IUserService userService) : IRequestHandler<DeletePartnerCommand>
{
    public Task Handle(DeletePartnerCommand request, CancellationToken cancellationToken)
        => userService.DeletePartnerAsync(request.UserId, cancellationToken);
}

public class DeletePartnerManagerCommandHandler(IUserService userService) : IRequestHandler<DeletePartnerManagerCommand>
{
    public Task Handle(DeletePartnerManagerCommand request, CancellationToken cancellationToken)
        => userService.DeletePartnerManagerAsync(request.UserId, cancellationToken);
}
