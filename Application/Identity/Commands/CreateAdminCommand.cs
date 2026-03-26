using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record CreateAdminCommand(string Email, string FirstName, string LastName) : IRequest<bool>;

public class CreateAdminCommandHandler(IAuthService authService) : IRequestHandler<CreateAdminCommand, bool>
{
    public async Task<bool> Handle(CreateAdminCommand request, CancellationToken cancellationToken)
        => await authService.CreateAdminAsync(request.Email, request.FirstName, request.LastName);
}
