using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Commands;

public record UploadPartnerAvatarCommand(int UserId, Stream Stream, string FileName) : IRequest<PartnerRepresentation>;

public class UploadPartnerAvatarCommandHandler(
    UserManager<User> userManager,
    IImageService imageService) : 
    IRequestHandler<UploadPartnerAvatarCommand, PartnerRepresentation>
{
    public async Task<PartnerRepresentation> Handle(UploadPartnerAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");

        var avatarPath = await imageService.UploadAvatarAsync(request.Stream, request.FileName, cancellationToken);
        user.AvatarPath = avatarPath;

        await userManager.UpdateAsync(user);
        
        return user.ToPartnerRepresentation();
    }
}
