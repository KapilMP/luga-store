using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Features.Users.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Queries;

public record GetPartnerProfileQuery(int UserId) : IRequest<PartnerRepresentation>;

public class GetPartnerProfileQueryHandler(UserManager<User> userManager) : 
    IRequestHandler<GetPartnerProfileQuery, PartnerRepresentation>
{
    public async Task<PartnerRepresentation> Handle(GetPartnerProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");
        return PartnerRepresentation.ToPartnerRepresentation(user);
    }
}
