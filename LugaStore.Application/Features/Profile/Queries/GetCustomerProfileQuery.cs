using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Features.Users.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Queries;

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

        return CustomerRepresentation.ToCustomerRepresentation(user);
    }
}
