using SedaWears.Application.Features.Users;
using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Common;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Profile.Queries;

public record GetAdminProfileQuery() : IRequest<AdminRepresentation>;

public class GetAdminProfileQueryHandler(UserManager<User> userManager, ICurrentUser currentUser) : 
    IRequestHandler<GetAdminProfileQuery, AdminRepresentation>
{
    public async Task<AdminRepresentation> Handle(GetAdminProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("Profile not found.");

        return (AdminRepresentation)user.ToUserRepresentation();
    }
}
