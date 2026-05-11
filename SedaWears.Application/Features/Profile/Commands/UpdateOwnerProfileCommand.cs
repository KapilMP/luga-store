using SedaWears.Application.Features.Users.Projections;
using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Common;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Profile.Commands;

public record UpdateOwnerProfileCommand(string FirstName, string LastName, string Phone, string? AvatarFileName) : IRequest<OwnerDto>;

public class UpdateOwnerProfileCommandHandler(UserManager<User> userManager, IS3Service s3Service, ICurrentUser currentUser) :
    IRequestHandler<UpdateOwnerProfileCommand, OwnerDto>
{
    public async Task<OwnerDto> Handle(UpdateOwnerProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("Profile not found.");

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

        return (OwnerDto)user.ToUserDto();
    }
}
