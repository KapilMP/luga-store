using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Profile.Queries;

public record GetCustomerProfileQuery() : IRequest<CustomerRepresentation>;

public class GetCustomerProfileQueryHandler(UserManager<User> userManager, ICurrentUser currentUser) : 
    IRequestHandler<GetCustomerProfileQuery, CustomerRepresentation>
{
    public async Task<CustomerRepresentation> Handle(GetCustomerProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var user = await userManager.Users
            .Include(u => u.Addresses)
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken) 
            ?? throw new NotFoundError("Profile not found.");

        return CustomerRepresentation.ToCustomerRepresentation(user);
    }
}
