using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record RegisterCommand(string Email, string Password, string FirstName, string LastName, string Phone) : IRequest<bool>;

public class RegisterCommandHandler(IAuthService authService) : IRequestHandler<RegisterCommand, bool>
{
    public async Task<bool> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        return await authService.RegisterAsync(request.Email, request.Password, request.FirstName, request.LastName, request.Phone);
    }
}
