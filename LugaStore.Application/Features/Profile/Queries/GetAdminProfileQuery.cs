using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Profile.Queries;

public record GetAdminProfileQuery() : IRequest<AdminRepresentation>;

public class GetAdminProfileQueryHandler(UserManager<User> userManager, ICurrentUser currentUser) : 
    IRequestHandler<GetAdminProfileQuery, AdminRepresentation>
{
    public async Task<AdminRepresentation> Handle(GetAdminProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundError("Profile not found.");
        return AdminRepresentation.ToAdminRepresentation(user);
    }
}
