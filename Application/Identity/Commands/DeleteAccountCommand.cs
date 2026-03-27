using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record DeleteAccountCommand : IRequest;

public class DeleteAccountCommandHandler(IUserService userService) : IRequestHandler<DeleteAccountCommand>
{
    public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
        => await userService.DeleteAccountAsync();
}
