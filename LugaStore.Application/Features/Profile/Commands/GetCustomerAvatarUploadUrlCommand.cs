using LugaStore.Application.Features.Profile.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Commands;

public record GetCustomerAvatarUploadUrlCommand(int UserId, string FileName, string ContentType) : IRequest<ImageUploadUrlResponse>;

public class GetCustomerAvatarUploadUrlCommandHandler(
    UserManager<User> userManager,
    IS3Service s3Service) :
    IRequestHandler<GetCustomerAvatarUploadUrlCommand, ImageUploadUrlResponse>
{
    public async Task<ImageUploadUrlResponse> Handle(GetCustomerAvatarUploadUrlCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");

        var extension = Path.GetExtension(request.FileName);
        var fileName = $"avatars/customer_{request.UserId}_{Guid.NewGuid()}{extension}";

        var uploadUrl = s3Service.GetPreSignedUrl(request.ContentType, fileName);

        return new ImageUploadUrlResponse(uploadUrl.ToString(), fileName);
    }
}
