using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Profile.Queries;

public record GetPartnerManagerProfileQuery(int UserId) : IRequest<PartnerManagerRepresentation>;

public class GetPartnerManagerProfileQueryHandler(UserManager<User> userManager) : 
    IRequestHandler<GetPartnerManagerProfileQuery, PartnerManagerRepresentation>
{
    public async Task<PartnerManagerRepresentation> Handle(GetPartnerManagerProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");
        return user.ToPartnerManagerRepresentation();
    }
}
