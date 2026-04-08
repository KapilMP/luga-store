using LugaStore.Application.Common.Settings;
using LugaStore.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using LugaStore.Application.Common.Exceptions;
using LugaStore.Application.Common.Interfaces;
using LugaStore.Application.Features.Users.Models;
using LugaStore.Domain.Common;
using LugaStore.Domain.Entities;

namespace LugaStore.Application.Features.Users.Queries;

public record GetUserQuery(int Id, string Role) : IRequest<UserRepresentation>;

public class GetUserHandler(IApplicationDbContext dbContext, UserManager<User> userManager) : IRequestHandler<GetUserQuery, UserRepresentation>
{
    public async Task<UserRepresentation> Handle(GetUserQuery request, CancellationToken ct)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString()) ?? throw new NotFoundError("User not found.");
        if (!await userManager.IsInRoleAsync(user, request.Role)) throw new NotFoundError($"{request.Role} not found.");

        if (request.Role == Roles.Customer)
        {
             user = await userManager.Users.Include(u => u.Addresses).FirstOrDefaultAsync(u => u.Id == request.Id, ct) ?? user;
             return CustomerRepresentation.ToCustomerRepresentation(user);
        }

        return UserRepresentation.ToUserRepresentation(user);
    }
}
