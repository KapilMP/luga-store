using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Commands;

public record DeleteAccountCommand(int UserId) : IRequest;

public class DeleteAccountCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<DeleteAccountCommand>
{
    public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("User not found.");
        await userManager.DeleteAsync(user);
    }
}
