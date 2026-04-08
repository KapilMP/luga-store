using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Profile.Commands;

public record UpdateCustomerProfileCommand(int UserId, string FirstName, string LastName, string Phone) : IRequest<CustomerRepresentation>;

public class UpdateCustomerProfileCommandHandler(UserManager<User> userManager) : 
    IRequestHandler<UpdateCustomerProfileCommand, CustomerRepresentation>
{
    public async Task<CustomerRepresentation> Handle(UpdateCustomerProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString()) ?? throw new NotFoundError("Profile not found.");

        user.FirstName = request.FirstName;
        user.LastName = request.LastName;
        user.PhoneNumber = request.Phone;

        await userManager.UpdateAsync(user);
        
        return CustomerRepresentation.ToCustomerRepresentation(user);
    }
}
