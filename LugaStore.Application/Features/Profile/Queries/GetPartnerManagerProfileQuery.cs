using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Profile.Queries;

public record GetPartnerManagerProfileQuery() : IRequest<PartnerManagerRepresentation>;

public class GetPartnerManagerProfileQueryHandler(UserManager<User> userManager, ICurrentUser currentUser) : 
    IRequestHandler<GetPartnerManagerProfileQuery, PartnerManagerRepresentation>
{
    public async Task<PartnerManagerRepresentation> Handle(GetPartnerManagerProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundError("Profile not found.");
        return PartnerManagerRepresentation.ToPartnerManagerRepresentation(user);
    }
}
