using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record SetUserActiveStatusCommand(int UserId, bool IsActive) : IRequest<bool>;

public class SetUserActiveStatusCommandHandler(IAuthService authService) : IRequestHandler<SetUserActiveStatusCommand, bool>
{
    public async Task<bool> Handle(SetUserActiveStatusCommand request, CancellationToken cancellationToken)
        => await authService.SetUserActiveStatusAsync(request.UserId, request.IsActive, cancellationToken);
}
