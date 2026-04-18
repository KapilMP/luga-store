using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

using LugaStore.Application.Common.Interfaces;

namespace LugaStore.Application.Features.Profile.Queries;

public record GetPartnerProfileQuery() : IRequest<PartnerRepresentation>;

public class GetPartnerProfileQueryHandler(UserManager<User> userManager, ICurrentUser currentUser) : 
    IRequestHandler<GetPartnerProfileQuery, PartnerRepresentation>
{
    public async Task<PartnerRepresentation> Handle(GetPartnerProfileQuery request, CancellationToken cancellationToken)
    {
        var userId = currentUser.Id!.Value;
        var user = await userManager.FindByIdAsync(userId.ToString()) ?? throw new NotFoundError("Profile not found.");
        return PartnerRepresentation.ToPartnerRepresentation(user);
    }
}
