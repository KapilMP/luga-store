using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Users.Commands;

public record DeleteInvitedAdminCommand(int Id) : IRequest;

public class DeleteInvitedAdminHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext) : IRequestHandler<DeleteInvitedAdminCommand>
{
    public async Task Handle(DeleteInvitedAdminCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && u.Role == UserRole.Admin && u.IsAdminInvitationAccepted != true, ct)
            ?? throw new NotFoundException("User not found.");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
            throw new BadRequestException(result.Errors.First().Description);
    }
}
