using SedaWears.Application.Features.Users;
using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Profile.Commands;

public record UpdateManagerProfileCommand(string FirstName, string LastName, string Phone, string? AvatarFileName) : IRequest<ManagerRepresentation>;

public class UpdateManagerProfileCommandHandler(UserManager<User> userManager, IS3Service s3Service, ICurrentUser currentUser) :
    IRequestHandler<UpdateManagerProfileCommand, ManagerRepresentation>
{
    public async Task<ManagerRepresentation> Handle(UpdateManagerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id;
        var user = await userManager.Users
            .Include(u => u.ShopMemberships)
            .ThenInclude(ms => ms.Shop)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken)
            ?? throw new NotFoundException("Profile not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.Phone;

        if (!string.IsNullOrEmpty(request.AvatarFileName) && request.AvatarFileName != user.AvatarFileName)
        {
            if (!string.IsNullOrEmpty(user.AvatarFileName))
            {
                await s3Service.DeleteFileAsync(user.AvatarFileName);
            }
            user.AvatarFileName = request.AvatarFileName;
        }

        await userManager.UpdateAsync(user);

        return (ManagerRepresentation)user.ToUserRepresentation();
    }
}
