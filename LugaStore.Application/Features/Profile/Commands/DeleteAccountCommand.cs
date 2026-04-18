using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Profile.Commands;

public record DeleteAccountCommand() : IRequest;

public class DeleteAccountCommandHandler(UserManager<User> userManager, ICurrentUser currentUser) : 
    IRequestHandler<DeleteAccountCommand>
{
    public async Task Handle(DeleteAccountCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundError("User not found.");
        await userManager.DeleteAsync(user);
    }
}
