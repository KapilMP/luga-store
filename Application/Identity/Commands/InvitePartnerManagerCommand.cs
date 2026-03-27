using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record InvitePartnerManagerCommand(string Email, string FirstName, string LastName, int PartnerId) : IRequest<bool>;

public class InvitePartnerManagerCommandHandler(IAuthService authService) : IRequestHandler<InvitePartnerManagerCommand, bool>
{
    public async Task<bool> Handle(InvitePartnerManagerCommand request, CancellationToken cancellationToken)
        => await authService.InvitePartnerManagerAsync(request.Email, request.FirstName, request.LastName, request.PartnerId, cancellationToken);
}
