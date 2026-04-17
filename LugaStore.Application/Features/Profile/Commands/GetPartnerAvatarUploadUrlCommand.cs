using LugaStore.Application.Features.Profile.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Settings;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Commands;

public record GetPartnerAvatarUploadUrlCommand(int UserId, string FileName, string ContentType) : IRequest<ImageUploadUrlResponse>;

public class GetPartnerAvatarUploadUrlCommandHandler(
    UserManager<User> userManager,
    IImageService imageService) : 
    IRequestHandler<GetPartnerAvatarUploadUrlCommand, ImageUploadUrlResponse>
{
    public async Task<ImageUploadUrlResponse> Handle(GetPartnerAvatarUploadUrlCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");

        var extension = Path.GetExtension(request.FileName);
        var fileName = $"avatars/partner_{request.UserId}_{Guid.NewGuid()}{extension}";
        
        var uploadUrl = await imageService.GetPresignedUrlAsync(fileName, request.ContentType, cancellationToken);
        
        return new ImageUploadUrlResponse(uploadUrl, fileName);
    }
}
