using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Users.Commands;

public record DeleteManagerCommand(int Id) : IRequest;

public class DeleteManagerHandler(UserManager<User> userManager, IUserCuckooFilter cuckooFilter) : IRequestHandler<DeleteManagerCommand>
{
    public async Task Handle(DeleteManagerCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString()) ?? throw new NotFoundException("Manager not found");
        if (user.Role != UserRole.Manager) throw new BadRequestException("User is not a Manager");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);

        await cuckooFilter.RemoveAsync(user.Email!, UserRole.Manager);
    }
}
