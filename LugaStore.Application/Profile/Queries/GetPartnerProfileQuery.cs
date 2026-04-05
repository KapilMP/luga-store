using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Profile.Queries;

public record GetPartnerProfileQuery(int UserId) : IRequest<PartnerRepresentation>;

public class GetPartnerProfileQueryHandler(UserManager<User> userManager) : 
    IRequestHandler<GetPartnerProfileQuery, PartnerRepresentation>
{
    public async Task<PartnerRepresentation> Handle(GetPartnerProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");
        return user.ToPartnerRepresentation();
    }
}
