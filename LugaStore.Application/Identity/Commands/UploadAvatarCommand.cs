using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Commands;

public record UploadAvatarCommand<T>(Stream Stream, string FileName) : IRequest<T> where T : BaseUserProfile;

public class UploadAvatarCommandHandler<T>(IUserService userService) : IRequestHandler<UploadAvatarCommand<T>, T> where T : BaseUserProfile
{
    public async Task<T> Handle(UploadAvatarCommand<T> request, CancellationToken cancellationToken)
        => await userService.UploadAvatarAsync<T>(request.Stream, request.FileName, cancellationToken);
}
