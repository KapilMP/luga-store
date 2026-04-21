using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Users;
using SedaWears.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace SedaWears.Application.Features.Profile.Queries;

public record GetManagerProfileQuery() : IRequest<ManagerRepresentation>;

public class GetManagerProfileQueryHandler(UserManager<User> userManager, ICurrentUser currentUser) : 
    IRequestHandler<GetManagerProfileQuery, ManagerRepresentation>
{
    public async Task<ManagerRepresentation> Handle(GetManagerProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Include(u => u.ManagedShops)
            .ThenInclude(ms => ms.Shop)
            .FirstOrDefaultAsync(u => u.Id == currentUser.Id, cancellationToken)
            ?? throw new UnauthorizedAccessException();
        
        return (ManagerRepresentation)user.ToUserRepresentation();
    }
}
