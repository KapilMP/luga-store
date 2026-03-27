using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record InvitePartnerCommand(string Email, string FirstName, string LastName) : IRequest<bool>;

public class InvitePartnerCommandHandler(IAuthService authService) : IRequestHandler<InvitePartnerCommand, bool>
{
    public async Task<bool> Handle(InvitePartnerCommand request, CancellationToken cancellationToken)
        => await authService.InvitePartnerAsync(request.Email, request.FirstName, request.LastName, cancellationToken);
}
