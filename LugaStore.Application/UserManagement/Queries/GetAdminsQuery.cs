using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Common.Configurations;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Common;

namespace LugaStore.Application.UserManagement.Queries;

public record GetAdminsQuery(int PageNumber = 1, int PageSize = 10, bool? Invited = null, bool? IsActive = null) : IRequest<PaginatedList<AdminRepresentation>>;

public class GetAdminsQueryHandler(IApplicationDbContext dbContext) : IRequestHandler<GetAdminsQuery, PaginatedList<AdminRepresentation>>
{
    public async Task<PaginatedList<AdminRepresentation>> Handle(GetAdminsQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Users
            .AsNoTracking()
            .Where(u => dbContext.UserRoles.Any(ur => ur.UserId == u.Id && 
                            dbContext.Roles.Any(r => r.Id == ur.RoleId && r.Name == Roles.Admin)));

        if (request.Invited.HasValue)
        {
            query = query.Where(u => u.EmailConfirmed != request.Invited.Value);
        }

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<AdminRepresentation>(
            items.Select(u => u.ToAdminRepresentation()).ToList(),
            count,
            request.PageNumber,
            request.PageSize);
    }
}
