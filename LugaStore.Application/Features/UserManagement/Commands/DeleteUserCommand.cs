using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.UserManagement.Commands;

public record DeleteUserCommand(int UserId) : IRequest;

public class DeleteUserCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("User not found.");
        await userManager.DeleteAsync(user);
    }
}
