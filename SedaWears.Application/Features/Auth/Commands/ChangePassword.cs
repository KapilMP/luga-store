using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;

using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Auth.Commands;

public record ChangePasswordCommand(string CurrentPassword, string NewPassword) : IRequest;

public class ChangePasswordHandler(UserManager<User> userManager, ICurrentUser currentUser) : IRequestHandler<ChangePasswordCommand>
{
    public async Task Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var userId = currentUser.Id!.Value;
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User not found.");
        var result = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);
    }
}
