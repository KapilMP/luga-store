using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record InviteAdminCommand(string Email, string FirstName, string LastName) : IRequest<bool>;

public class InviteAdminCommandHandler(IUserService userService) : IRequestHandler<InviteAdminCommand, bool>
{
    public async Task<bool> Handle(InviteAdminCommand request, CancellationToken cancellationToken)
        => await userService.InviteAdminAsync(request.Email, request.FirstName, request.LastName, cancellationToken);
}
