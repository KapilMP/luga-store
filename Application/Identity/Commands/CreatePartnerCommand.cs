using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record CreatePartnerCommand(string Email, string FirstName, string LastName) : IRequest<bool>;

public class CreatePartnerCommandHandler(IAuthService authService) : IRequestHandler<CreatePartnerCommand, bool>
{
    public async Task<bool> Handle(CreatePartnerCommand request, CancellationToken cancellationToken)
        => await authService.CreatePartnerAsync(request.Email, request.FirstName, request.LastName);
}
