using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Auth.Commands;

public record ResetPasswordCommand(string Email, string Token, string NewPassword) : IRequest;

public class ResetPasswordHandler(UserManager<User> userManager) : IRequestHandler<ResetPasswordCommand>
{
    public async Task Handle(ResetPasswordCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(request.Email) ?? throw new NotFoundException("User not found");
        var result = await userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);
    }
}
