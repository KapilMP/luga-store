using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Auth.Commands;

public record ConfirmEmailCommand(int UserId, string Token) : IRequest;

public class ConfirmEmailHandler(UserManager<User> userManager) : IRequestHandler<ConfirmEmailCommand>
{
    public async Task Handle(ConfirmEmailCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("User not found");
        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded) throw new BadRequestError("Confirmation failed");
    }
}
