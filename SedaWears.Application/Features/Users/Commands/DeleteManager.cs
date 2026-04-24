using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Users.Commands;

public record DeleteManagerCommand(int Id) : IRequest;

public class DeleteManagerHandler(
    UserManager<User> userManager,
    IApplicationDbContext dbContext,
    IUserCuckooFilter cuckooFilter) : IRequestHandler<DeleteManagerCommand>
{
    public async Task Handle(DeleteManagerCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && u.Role == UserRole.Manager, ct)
            ?? throw new NotFoundException("Manager not found");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);

        await cuckooFilter.RemoveAsync(user.Email!, UserRole.Manager);
    }
}
