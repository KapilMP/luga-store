using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record CreatePartnerManagerCommand(string Email, string FirstName, string LastName) : IRequest<bool>;

public class CreatePartnerManagerCommandHandler(IAuthService authService) : IRequestHandler<CreatePartnerManagerCommand, bool>
{
    public async Task<bool> Handle(CreatePartnerManagerCommand request, CancellationToken cancellationToken)
        => await authService.CreatePartnerManagerAsync(request.Email, request.FirstName, request.LastName);
}
