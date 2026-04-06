using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Common.Models;
using LugaStore.Application.Common.Settings;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Domain.Common;

namespace LugaStore.Application.Features.UserManagement.Queries;

public record GetCustomersQuery(int PageNumber = 1, int PageSize = 10, bool? IsActive = null) : IRequest<PaginatedList<CustomerRepresentation>>;

public class GetCustomersQueryHandler(IApplicationDbContext dbContext) : IRequestHandler<GetCustomersQuery, PaginatedList<CustomerRepresentation>>
{
    public async Task<PaginatedList<CustomerRepresentation>> Handle(GetCustomersQuery request, CancellationToken cancellationToken)
    {
        var query = dbContext.Users
            .AsNoTracking()
            .Include(u => u.Addresses)
            .Where(u => dbContext.UserRoles.Any(ur => ur.UserId == u.Id && 
                            dbContext.Roles.Any(r => r.Id == ur.RoleId && r.Name == Roles.Customer)));

        if (request.IsActive.HasValue)
        {
            query = query.Where(u => u.IsActive == request.IsActive.Value);
        }

        var count = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedList<CustomerRepresentation>(
            items.Select(u => u.ToCustomerRepresentation()).ToList(),
            count,
            request.PageNumber,
            request.PageSize);
    }
}
