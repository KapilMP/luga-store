using LugaStore.Application.Features.Profile.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Settings;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Commands;

public record GetPartnerManagerAvatarUploadUrlCommand(string FileName, string ContentType) : IRequest<ImageUploadUrlResponse>;

public class GetPartnerManagerAvatarUploadUrlCommandHandler(
    UserManager<User> userManager,
    IS3Service s3Service,
    ICurrentUser currentUser) : 
    IRequestHandler<GetPartnerManagerAvatarUploadUrlCommand, ImageUploadUrlResponse>
{
    public async Task<ImageUploadUrlResponse> Handle(GetPartnerManagerAvatarUploadUrlCommand request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundError("Profile not found.");

        var extension = Path.GetExtension(request.FileName);
        var fileName = $"avatars/manager_{userId}_{Guid.NewGuid()}{extension}";
        
        var uploadUrl = s3Service.GetPreSignedUrl(request.ContentType, fileName);
        
        return new ImageUploadUrlResponse(uploadUrl.ToString(), fileName);
    }
}
