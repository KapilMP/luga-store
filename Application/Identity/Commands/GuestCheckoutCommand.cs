using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record GuestCheckoutCommand(string Email, string FirstName, string LastName, string Phone) : IRequest<bool>;

public class GuestCheckoutCommandHandler(IAuthService authService) : IRequestHandler<GuestCheckoutCommand, bool>
{
    public async Task<bool> Handle(GuestCheckoutCommand request, CancellationToken cancellationToken)
        => await authService.GuestCheckoutAsync(request.Email, request.FirstName, request.LastName, request.Phone, cancellationToken);
}
