using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Commands;

public record UpdateProfileCommand<T>(string FirstName, string LastName, string Phone) : IRequest<T> where T : BaseUserProfile;

public class UpdateProfileCommandHandler<T>(IUserService userService) : IRequestHandler<UpdateProfileCommand<T>, T> where T : BaseUserProfile
{
    public async Task<T> Handle(UpdateProfileCommand<T> request, CancellationToken cancellationToken)
        => await userService.UpdateProfileAsync<T>(request.FirstName, request.LastName, request.Phone);
}
