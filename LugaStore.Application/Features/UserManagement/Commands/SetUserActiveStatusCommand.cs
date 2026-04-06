using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.UserManagement.Commands;

public record SetUserActiveStatusCommand(int UserId, bool IsActive) : IRequest;

public class SetUserActiveStatusCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<SetUserActiveStatusCommand>
{
    public async Task Handle(SetUserActiveStatusCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("User not found.");
        user.IsActive = request.IsActive;
        await userManager.UpdateAsync(user);
    }
}
