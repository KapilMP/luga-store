using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Enums;
using SedaWears.Domain.Entities;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Users.Queries;

public record GetUserQuery(UserRole? Role = null, int? Id = null) : IRequest<BaseUserRepresentation>;

public class GetUserHandler(
    UserManager<User> userManager,
    ICurrentUser currentUser,
    IOriginContext originContext) : IRequestHandler<GetUserQuery, BaseUserRepresentation>
{
    public async Task<BaseUserRepresentation> Handle(GetUserQuery request, CancellationToken ct)
    {
        var role = request.Role ?? originContext.CurrentRole;
        var userId = request.Id ?? currentUser.Id!.Value;

        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundException("User not found.");

        if (user.Role != role) throw new NotFoundException($"{role} not found.");

        var query = userManager.Users;

        if (role == UserRole.Customer) query = query.Include(u => u.Addresses);
        if (role == UserRole.Manager) query = query.Include(u => u.ManagedShops).ThenInclude(ms => ms.Shop);

        user = await query.FirstOrDefaultAsync(u => u.Id == userId, ct) ?? user;

        return user.ToUserRepresentation();
    }
}
