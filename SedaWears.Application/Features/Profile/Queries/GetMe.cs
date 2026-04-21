using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Enums;
using SedaWears.Domain.Entities;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Application.Features.Users;

namespace SedaWears.Application.Features.Profile.Queries;

public record GetMeQuery : IRequest<BaseUserRepresentation>;

public class GetMeHandler(
    UserManager<User> userManager,
    ICurrentUser currentUser) : IRequestHandler<GetMeQuery, BaseUserRepresentation>
{
    public async Task<BaseUserRepresentation> Handle(GetMeQuery request, CancellationToken ct)
    {
        var userId = currentUser.Id;
        var role = currentUser.Role;

        var query = userManager.Users.AsQueryable();

        if (role == UserRole.Customer)
        {
            query = query.Include(u => u.Addresses);
        }
        else if (role == UserRole.Manager)
        {
            query = query.Include(u => u.ManagedShops).ThenInclude(ms => ms.Shop);
        }

        var user = await query.FirstOrDefaultAsync(u => u.Id == userId && u.Role == role, ct)
            ?? throw new NotFoundException("User not found.");

        return user.ToUserRepresentation();
    }
}
