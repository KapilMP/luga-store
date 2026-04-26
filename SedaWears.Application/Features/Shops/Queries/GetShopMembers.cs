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

        var shopMembers = await sortedQuery
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Include(sm => sm.User)
            .ToListAsync(ct);

        var mappedMembers = shopMembers
            .Select(sm => sm.User.ToUserRepresentation(sm.CreatedAt))
            .ToList();

        return new PaginatedList<BaseUserRepresentation>(mappedMembers, totalCount, request.PageNumber, request.PageSize);
    }
}
