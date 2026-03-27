using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record InvitePartnerCommand(string Email, string FirstName, string LastName) : IRequest<bool>;

public class InvitePartnerCommandHandler(IUserService userService) : IRequestHandler<InvitePartnerCommand, bool>
{
    public async Task<bool> Handle(InvitePartnerCommand request, CancellationToken cancellationToken)
        => await userService.InvitePartnerAsync(request.Email, request.FirstName, request.LastName, cancellationToken);
}
