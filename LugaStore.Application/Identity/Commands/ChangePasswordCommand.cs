using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<bool>;

public class ChangePasswordCommandHandler(IAuthService authService, ICurrentUser currentUser) : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        => await authService.ChangePasswordAsync(currentUser.UserId!, request.CurrentPassword, request.NewPassword, cancellationToken);
}
