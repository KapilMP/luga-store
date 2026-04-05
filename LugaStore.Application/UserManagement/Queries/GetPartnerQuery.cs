using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Common;

namespace LugaStore.Application.UserManagement.Queries;

public record GetPartnerQuery(int Id) : IRequest<PartnerRepresentation>;

public class GetPartnerQueryHandler(IApplicationDbContext dbContext) : IRequestHandler<GetPartnerQuery, PartnerRepresentation>
{
    public async Task<PartnerRepresentation> Handle(GetPartnerQuery request, CancellationToken cancellationToken)
    {
        var partner = await dbContext.Users
            .AsNoTracking()
            .Where(u => u.Id == request.Id && 
                        dbContext.UserRoles.Any(ur => ur.UserId == u.Id && 
                            dbContext.Roles.Any(r => r.Id == ur.RoleId && r.Name == Roles.Partner)))
            .FirstOrDefaultAsync(cancellationToken);

        return partner == null ? throw new NotFoundError("Partner not found.") : partner.ToPartnerRepresentation();
    }
}
