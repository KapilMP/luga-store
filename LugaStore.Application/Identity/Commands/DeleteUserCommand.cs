using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record DeleteUserCommand(int Id) : IRequest<bool>;

public class DeleteUserCommandHandler(IUserService userService) : IRequestHandler<DeleteUserCommand, bool>
{
    public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        => await userService.DeleteUserAsync(request.Id, cancellationToken);
}
