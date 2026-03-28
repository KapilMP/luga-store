using MediatR;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Models;

namespace LugaStore.Application.Identity.Queries;

public record GetProfileQuery<T> : IRequest<T> where T : BaseUserProfile;

public class GetProfileQueryHandler<T>(IUserService userService) : IRequestHandler<GetProfileQuery<T>, T> where T : BaseUserProfile
{
    public async Task<T> Handle(GetProfileQuery<T> request, CancellationToken cancellationToken)
        => await userService.GetProfileAsync<T>();
}
