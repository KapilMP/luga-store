using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Features.Users.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Commands;

public record UploadPartnerManagerAvatarCommand(int UserId, Stream Stream, string FileName) : IRequest<PartnerManagerRepresentation>;

public class UploadPartnerManagerAvatarCommandHandler(
    UserManager<User> userManager,
    IImageService imageService) : 
    IRequestHandler<UploadPartnerManagerAvatarCommand, PartnerManagerRepresentation>
{
    public async Task<PartnerManagerRepresentation> Handle(UploadPartnerManagerAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");

        var avatarPath = await imageService.UploadAvatarAsync(request.Stream, request.FileName, cancellationToken);
        user.AvatarPath = avatarPath;

        await userManager.UpdateAsync(user);
        
        return PartnerManagerRepresentation.ToPartnerManagerRepresentation(user);
    }
}
