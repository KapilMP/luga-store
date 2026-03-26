using MediatR;
using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Identity.Commands;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest<bool>;

public class ChangePasswordCommandHandler(IAuthService authService, IUserService userService) : IRequestHandler<ChangePasswordCommand, bool>
{
    public async Task<bool> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        => await authService.ChangePasswordAsync(userService.UserId!, request.CurrentPassword, request.NewPassword);
}
