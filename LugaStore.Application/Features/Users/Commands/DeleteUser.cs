using LugaStore.Application.Common.Settings;
using LugaStore.Application.Features.Users.Models;
using MediatR;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Users.Commands;

public record DeleteUserCommand(int UserId, string Role) : IRequest;

public class DeleteUserValidator : AbstractValidator<DeleteUserCommand>
{
    public DeleteUserValidator()
    {
        RuleFor(x => x.UserId).GreaterThan(0);
        RuleFor(x => x.Role).NotEmpty();
    }
}

public class DeleteUserHandler(UserManager<User> userManager) : IRequestHandler<DeleteUserCommand>
{
    public async Task Handle(DeleteUserCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("User not found.");
        if (!await userManager.IsInRoleAsync(user, request.Role)) throw new BadRequestError($"User is not in role {request.Role}.");

        if (request.Role == Roles.Admin)
        {
            var roles = await userManager.GetRolesAsync(user);
            if (roles.Count > 1) { await userManager.RemoveFromRoleAsync(user, Roles.Admin); return; }
        }
        
        await userManager.DeleteAsync(user);
    }
}
