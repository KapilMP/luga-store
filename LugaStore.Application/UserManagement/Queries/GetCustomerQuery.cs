using MediatR;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Common;

namespace LugaStore.Application.UserManagement.Queries;

public record GetCustomerQuery(int Id) : IRequest<CustomerRepresentation>;

public class GetCustomerQueryHandler(IApplicationDbContext dbContext) : IRequestHandler<GetCustomerQuery, CustomerRepresentation>
{
    public async Task<CustomerRepresentation> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = await dbContext.Users
            .AsNoTracking()
            .Include(u => u.Addresses)
            .Where(u => u.Id == request.Id && 
                        dbContext.UserRoles.Any(ur => ur.UserId == u.Id && 
                            dbContext.Roles.Any(r => r.Id == ur.RoleId && r.Name == Roles.Customer)))
            .FirstOrDefaultAsync(cancellationToken);

        return customer == null ? throw new NotFoundError("Customer not found.") : customer.ToCustomerRepresentation();
    }
}
