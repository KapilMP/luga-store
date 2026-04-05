using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Profile.Queries;

public record GetCustomerProfileQuery(int UserId) : IRequest<CustomerRepresentation>;

public class GetCustomerProfileQueryHandler(UserManager<User> userManager) : 
    IRequestHandler<GetCustomerProfileQuery, CustomerRepresentation>
{
    public async Task<CustomerRepresentation> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == request.UserId, cancellationToken) 
            ?? throw new NotFoundError("Profile not found.");

        return user.ToCustomerRepresentation();
    }
}
