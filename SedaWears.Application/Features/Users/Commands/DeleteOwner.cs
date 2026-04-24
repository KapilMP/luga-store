using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Users.Commands;

public record DeleteOwnerCommand(int Id) : IRequest;

public class DeleteOwnerHandler(
    UserManager<User> userManager, 
    IApplicationDbContext dbContext,
    IUserCuckooFilter cuckooFilter) : IRequestHandler<DeleteOwnerCommand>
{
    public async Task Handle(DeleteOwnerCommand request, CancellationToken ct)
    {
        var user = await dbContext.Users
            .FirstOrDefaultAsync(u => u.Id == request.Id && u.Role == UserRole.Owner, ct)
            ?? throw new NotFoundException("Owner not found");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);

        await cuckooFilter.RemoveAsync(user.Email!, UserRole.Owner);
    }
}
