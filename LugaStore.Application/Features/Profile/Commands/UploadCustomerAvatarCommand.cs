using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Commands;

public record UploadCustomerAvatarCommand(int UserId, Stream Stream, string FileName) : IRequest<CustomerRepresentation>;

public class UploadCustomerAvatarCommandHandler(
    UserManager<User> userManager,
    IImageService imageService) : 
    IRequestHandler<UploadCustomerAvatarCommand, CustomerRepresentation>
{
    public async Task<CustomerRepresentation> Handle(UploadCustomerAvatarCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");

        var avatarPath = await imageService.UploadAvatarAsync(request.Stream, request.FileName, cancellationToken);
        user.AvatarPath = avatarPath;

        await userManager.UpdateAsync(user);
        
        return user.ToCustomerRepresentation();
    }
}
