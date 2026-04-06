using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Auth.Commands;

public record ConfirmEmailCommand(string UserId, string Token) : IRequest<bool>;

public class ConfirmEmailCommandHandler(UserManager<User> userManager) :
    IRequestHandler<ConfirmEmailCommand, bool>
{
    public async Task<bool> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId);
        if (user == null) return false;
        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        return result.Succeeded;
    }
}
