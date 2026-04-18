using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Profile.Commands;

public record UpdatePartnerManagerProfileCommand(string FirstName, string LastName, string Phone, string? AvatarFileName) : IRequest<PartnerManagerRepresentation>;

public class UpdatePartnerManagerProfileCommandHandler(UserManager<User> userManager, IS3Service s3Service, ICurrentUser currentUser) : 
    IRequestHandler<UpdatePartnerManagerProfileCommand, PartnerManagerRepresentation>
{
    public async Task<PartnerManagerRepresentation> Handle(UpdatePartnerManagerProfileCommand request, CancellationToken cancellationToken)
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
        
        return PartnerManagerRepresentation.ToPartnerManagerRepresentation(user);
    }
}
