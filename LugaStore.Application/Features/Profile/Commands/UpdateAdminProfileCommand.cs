using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Profile.Commands;

public record UpdateAdminProfileCommand(string FirstName, string LastName, string Phone, string? AvatarFileName) : IRequest<AdminRepresentation>;

public class UpdateAdminProfileCommandHandler(UserManager<User> userManager, IS3Service s3Service, ICurrentUser currentUser) : 
    IRequestHandler<UpdateAdminProfileCommand, AdminRepresentation>
{
    public async Task<AdminRepresentation> Handle(UpdateAdminProfileCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundError("Profile not found.");

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
        
        return AdminRepresentation.ToAdminRepresentation(user);
    }
}
