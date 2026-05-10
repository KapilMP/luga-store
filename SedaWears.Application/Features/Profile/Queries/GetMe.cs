using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using SedaWears.Application.Common.Exceptions;
using SedaWears.Domain.Enums;
using SedaWears.Domain.Entities;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Application.Features.Users.Projections;

namespace SedaWears.Application.Features.Profile.Queries;

public record GetMeQuery : IRequest<BaseUserRepresentation>;

public class GetMeHandler(
    IUserService userService,
    ICurrentUser currentUser) : IRequestHandler<GetMeQuery, BaseUserRepresentation>
{
    public async Task<BaseUserRepresentation> Handle(GetMeQuery request, CancellationToken ct)
    {
        return await userService.GetUserByIdAndRoleAsync<BaseUserRepresentation>(
            currentUser.Id!.Value,
            currentUser.Role!.Value,
            ct);
    }
}
