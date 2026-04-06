using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Queries;

public record GetAdminProfileQuery(int UserId) : IRequest<AdminRepresentation>;

public class GetAdminProfileQueryHandler(UserManager<User> userManager) : 
    IRequestHandler<GetAdminProfileQuery, AdminRepresentation>
{
    public async Task<AdminRepresentation> Handle(GetAdminProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");
        return user.ToAdminRepresentation();
    }
}
