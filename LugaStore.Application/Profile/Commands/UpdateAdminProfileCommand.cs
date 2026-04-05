using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Mappings;
using LugaStore.Application.UserManagement.Models;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Profile.Commands;

public record UpdateAdminProfileCommand(int UserId, string FirstName, string LastName, string Phone) : IRequest<AdminRepresentation>;

public class UpdateAdminProfileCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<UpdateAdminProfileCommand, AdminRepresentation>
{
    public async Task<AdminRepresentation> Handle(UpdateAdminProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.Phone;

        await userManager.UpdateAsync(user);
        
        return user.ToAdminRepresentation();
    }
}
