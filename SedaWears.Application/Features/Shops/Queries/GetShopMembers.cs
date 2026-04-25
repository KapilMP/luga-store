using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Users.Models;
using SedaWears.Domain.Enums;
using SedaWears.Application.Features.Users;
using SedaWears.Domain.Entities;

namespace SedaWears.Application.Features.Shops.Queries;

public record GetShopMembersQuery(
    int ShopId,
    UserRole? Role = null,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = "createdAt",
    string? SortOrder = "desc",
    bool? IsInvited = null) : IRequest<PaginatedList<BaseUserRepresentation>>;

public class GetShopMembersHandler(IApplicationDbContext dbContext) : IRequestHandler<GetShopMembersQuery, PaginatedList<BaseUserRepresentation>>
{
    public async Task<PaginatedList<BaseUserRepresentation>> Handle(GetShopMembersQuery request, CancellationToken ct)
    {
        var query = dbContext.ShopMembers
            .AsNoTracking()
            .Include(sm => sm.User)
            .Where(sm => sm.ShopId == request.ShopId);

        if (request.Role.HasValue)
            query = query.Where(sm => sm.User.Role == request.Role);

        if (request.IsInvited.HasValue)
            query = query.Where(sm => sm.IsInvitationAccepted == !request.IsInvited.Value);

        var userQuery = query.Select(sm => sm.User)
            .Include(u => u.ShopMemberships)
            .ThenInclude(sm => sm.Shop);

        IQueryable<User> sortedUserQuery = request.SortBy?.ToLower() switch
        {
            "firstname" => request.SortOrder?.ToLower() == "asc" ? userQuery.OrderBy(u => u.FirstName) : userQuery.OrderByDescending(u => u.FirstName),
            "lastname" => request.SortOrder?.ToLower() == "asc" ? userQuery.OrderBy(u => u.LastName) : userQuery.OrderByDescending(u => u.LastName),
            "email" => request.SortOrder?.ToLower() == "asc" ? userQuery.OrderBy(u => u.Email) : userQuery.OrderByDescending(u => u.Email),
            _ => request.SortOrder?.ToLower() == "asc" ? userQuery.OrderBy(u => u.CreatedAt) : userQuery.OrderByDescending(u => u.CreatedAt)
        };

        var totalCount = await sortedUserQuery.CountAsync(ct);

        var users = await sortedUserQuery
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(ct);

        var mappedMembers = users.Select(u => u.ToUserRepresentation()).ToList();

        return new PaginatedList<BaseUserRepresentation>(mappedMembers, totalCount, request.PageNumber, request.PageSize);
    }
}
