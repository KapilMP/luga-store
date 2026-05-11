using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Domain.Enums;
using SedaWears.Application.Features.Users.Projections;
using SedaWears.Domain.Entities;

using SedaWears.Application.Common.Validators;

namespace SedaWears.Application.Features.Shops.Queries;

public record GetShopMembersQuery(
    int ShopId,
    UserRole? Role = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = "createdAt",
    string? SortOrder = "desc",
    bool? IsInvited = null) : IRequest<PaginatedList<BaseUserDto>>, IPaginatedQuery;

public class GetShopMembersValidator : PaginatedQueryValidator<GetShopMembersQuery> { }

public class GetShopMembersHandler(IApplicationDbContext dbContext) : IRequestHandler<GetShopMembersQuery, PaginatedList<BaseUserDto>>
{
    public async Task<PaginatedList<BaseUserDto>> Handle(GetShopMembersQuery request, CancellationToken ct)
    {
        var query = dbContext.ShopMembers
            .AsNoTracking()
            .Where(sm => sm.ShopId == request.ShopId);

        if (request.Role.HasValue)
            query = query.Where(sm => sm.User.Role == request.Role);

        if (request.IsInvited.HasValue)
        {
            var acceptedStatus = !request.IsInvited.Value;
            query = query.Where(sm => sm.IsInvitationAccepted == acceptedStatus);
        }

        IQueryable<ShopMember> sortedQuery = request.SortBy?.ToLower() switch
        {
            "name" => request.SortOrder?.ToLower() == "asc" ? query.OrderBy(sm => sm.User.FirstName) : query.OrderByDescending(sm => sm.User.FirstName),
            "email" => request.SortOrder?.ToLower() == "asc" ? query.OrderBy(sm => sm.User.Email) : query.OrderByDescending(sm => sm.User.Email),
            _ => request.SortOrder?.ToLower() == "asc" ? query.OrderBy(sm => sm.CreatedAt) : query.OrderByDescending(sm => sm.CreatedAt)
        };

        var totalCount = await sortedQuery.CountAsync(ct);

        var pagedMembers = sortedQuery
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize);

        List<BaseUserDto> mappedMembers;

        if (request.Role == UserRole.Owner)
        {
            mappedMembers = (await pagedMembers.Select(sm => sm.User).ProjectToOwner().ToListAsync(ct))
                .Cast<BaseUserDto>().ToList();
        }
        else if (request.Role == UserRole.Manager)
        {
            mappedMembers = (await pagedMembers.Select(sm => sm.User).ProjectToManager().ToListAsync(ct))
                .Cast<BaseUserDto>().ToList();
        }
        else
        {
            // For mixed roles in ShopMembers, we fetch and map
            var members = await pagedMembers
                .Include(sm => sm.User)
                .ThenInclude(u => u.ShopMemberships)
                .ThenInclude(ms => ms.Shop)
                .ToListAsync(ct);

            mappedMembers = members.Select(sm => sm.User.ToUserDto(sm.CreatedAt)).ToList();
        }

        return new PaginatedList<BaseUserDto>(mappedMembers, totalCount, request.PageNumber, request.PageSize);
    }
}
