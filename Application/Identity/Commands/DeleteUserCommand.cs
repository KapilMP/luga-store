using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record DeleteUserCommand(int Id) : IRequest<bool>;

public class DeleteUserCommandHandler(IAuthService authService) : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        => await authService.DeleteUserAsync(request.Id, cancellationToken);
}
