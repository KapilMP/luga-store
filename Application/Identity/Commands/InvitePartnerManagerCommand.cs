using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record InvitePartnerManagerCommand(string Email, string FirstName, string LastName) : IRequest<bool>;

public class InvitePartnerManagerCommandHandler(IUserService userService) : IRequestHandler<InvitePartnerManagerCommand, bool>
{
    public async Task<bool> Handle(InvitePartnerManagerCommand request, CancellationToken cancellationToken)
        => await userService.InvitePartnerManagerAsync(request.Email, request.FirstName, request.LastName, cancellationToken);
}
