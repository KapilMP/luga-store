using LugaStore.Application.Common.Settings;
using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Features.Users.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Users.Queries;

public record GetUsersQuery(string Role, int? PartnerId = null) : IRequest<List<UserRepresentation>>;

public class GetUsersHandler(IApplicationDbContext dbContext, UserManager<User> userManager) : IRequestHandler<GetUsersQuery, List<UserRepresentation>>
{
    public async Task<List<UserRepresentation>> Handle(GetUsersQuery request, CancellationToken ct)
    {
        var users = await userManager.GetUsersInRoleAsync(request.Role);
        var query = users.AsQueryable();

        if (request.PartnerId.HasValue)
        {
            var managerIds = await dbContext.PartnerManagers
                .Where(pm => pm.PartnerId == request.PartnerId)
                .Select(pm => pm.ManagerId)
                .ToListAsync(ct);
            query = query.Where(u => managerIds.Contains(u.Id));
        }

        return query.Select(UserRepresentation.ToUserRepresentation).ToList();
    }
}
