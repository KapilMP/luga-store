using MediatR;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Users;
using SedaWears.Domain.Entities;
using SedaWears.Domain.Enums;

namespace SedaWears.Application.Features.Profile.Queries;

public record GetOwnerProfileQuery() : IRequest<OwnerRepresentation>;

public class GetOwnerProfileQueryHandler(UserManager<User> userManager, ICurrentUser currentUser) : 
    IRequestHandler<GetOwnerProfileQuery, OwnerRepresentation>
{
    public async Task<OwnerRepresentation> Handle(GetOwnerProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(currentUser.Id.ToString()!) ?? throw new UnauthorizedAccessException();
        
        return (OwnerRepresentation)UserMapper.ToUserRepresentation(user);
    }
}
