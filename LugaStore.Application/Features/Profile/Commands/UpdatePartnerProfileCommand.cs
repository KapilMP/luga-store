using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Profile.Commands;

public record UpdatePartnerProfileCommand(string FirstName, string LastName, string Phone, string? AvatarFileName) : IRequest<PartnerRepresentation>;

public class UpdatePartnerProfileCommandHandler(UserManager<User> userManager, IS3Service s3Service, ICurrentUser currentUser) : 
    IRequestHandler<UpdatePartnerProfileCommand, PartnerRepresentation>
{
    public async Task<PartnerRepresentation> Handle(UpdatePartnerProfileCommand request, CancellationToken cancellationToken)
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
        
        return PartnerRepresentation.ToPartnerRepresentation(user);
    }
}
