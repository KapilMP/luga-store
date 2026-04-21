using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Users.Commands;

public record DeleteOwnerCommand(int Id) : IRequest;

public class DeleteOwnerHandler(
    UserManager<User> userManager, 
    IUserCuckooFilter cuckooFilter) : IRequestHandler<DeleteOwnerCommand>
{
    public async Task Handle(DeleteOwnerCommand request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString()) ?? throw new NotFoundException("Owner not found");
        
        if (user.Role != UserRole.Owner)
            throw new BadRequestException("User is not an Owner");

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded) throw new BadRequestException(result.Errors.First().Description);

        await cuckooFilter.RemoveAsync(user.Email!, UserRole.Owner);
    }
}
