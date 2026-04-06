using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.Features.UserManagement.Models;
using LugaStore.Domain.Common;

namespace LugaStore.Application.Features.UserManagement.Queries;

public record GetAdminQuery(int Id) : IRequest<AdminRepresentation>;

public class GetAdminQueryHandler(IApplicationDbContext dbContext) : IRequestHandler<GetAdminQuery, AdminRepresentation>
{
    public async Task<AdminRepresentation> Handle(GetAdminQuery request, CancellationToken cancellationToken)
    {
        var admin = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == request.Id && 
                        dbContext.UserRoles.Any(ur => ur.UserId == u.Id && 
                            dbContext.Roles.Any(r => r.Id == ur.RoleId && r.Name == Roles.Admin)))
            .FirstOrDefaultAsync(cancellationToken);

        return admin == null ? throw new NotFoundError("Admin not found.") : admin.ToAdminRepresentation();
    }
}
