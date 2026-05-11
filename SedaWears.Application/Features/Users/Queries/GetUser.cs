using SedaWears.Application.Features.Users.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Enums;
using SedaWears.Domain.Entities;
using SedaWears.Application.Common.Interfaces;

using SedaWears.Application.Features.Users.Projections;

namespace SedaWears.Application.Features.Users.Queries;

public record GetUserQuery(UserRole? Role = null, int? Id = null) : IRequest<BaseUserDto>;

public class GetUserHandler(
    UserManager<User> userManager,
    ICurrentUser currentUser,
    IOriginContext originContext) : IRequestHandler<GetUserQuery, BaseUserDto>
{
    public async Task<BaseUserDto> Handle(GetUserQuery request, CancellationToken ct)
    {
        var role = request.Role ?? originContext.CurrentRole;
        var userId = request.Id ?? currentUser.Id!.Value;

        var query = userManager.Users.AsNoTracking().Where(u => u.Id == userId && u.Role == role);

        BaseUserDto? userDto = role switch
        {
            UserRole.Admin => await query.ProjectToAdmin().FirstOrDefaultAsync(ct),
            UserRole.Owner => await query.ProjectToOwner().FirstOrDefaultAsync(ct),
            UserRole.Manager => await query.ProjectToManager().FirstOrDefaultAsync(ct),
            UserRole.Customer => await query.ProjectToCustomer().FirstOrDefaultAsync(ct),
            _ => throw new ArgumentOutOfRangeException()
        };

        return userDto ?? throw new NotFoundException($"{role} not found.");
    }
}
