using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Commands;

public record UpdatePartnerProfileCommand(int UserId, string FirstName, string LastName, string Phone) : IRequest<PartnerRepresentation>;

public class UpdatePartnerProfileCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<UpdatePartnerProfileCommand, PartnerRepresentation>
{
    public async Task<PartnerRepresentation> Handle(UpdatePartnerProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.Phone;

        await userManager.UpdateAsync(user);
        
        return PartnerRepresentation.ToPartnerRepresentation(user);
    }
}
