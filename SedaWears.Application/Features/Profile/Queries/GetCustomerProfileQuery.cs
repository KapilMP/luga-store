using SedaWears.Application.Features.Users;
using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Common;
using SedaWears.Application.Common.Interfaces;

namespace SedaWears.Application.Features.Profile.Queries;

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
            ?? throw new NotFoundException("Profile not found.");

        return (CustomerRepresentation)user.ToUserRepresentation();
    }
}
