using MediatR;
using Microsoft.EntityFrameworkCore;
using SedaWears.Application.Common.Interfaces;
using SedaWears.Application.Common.Models;
using SedaWears.Application.Features.Users.Models;

namespace SedaWears.Application.Features.Shops.Queries;

public record GetShopOwnersQuery(
    int ShopId,
    int PageNumber = 1,
    int PageSize = 10,
    string? SortBy = "createdAt",
    string? SortOrder = "desc") : IRequest<PaginatedList<OwnerRepresentation>>;

public class GetShopOwnersHandler(IApplicationDbContext dbContext) : IRequestHandler<GetShopOwnersQuery, PaginatedList<OwnerRepresentation>>
{
    public async Task<PaginatedList<OwnerRepresentation>> Handle(GetShopOwnersQuery request, CancellationToken ct)
    {
        var query = dbContext.ShopOwners
            .AsNoTracking()
            .Where(so => so.ShopId == request.ShopId)
            .Select(so => so.Owner);

        // Sorting
        query = request.SortBy?.ToLower() switch
        {
            "firstname" => request.SortOrder?.ToLower() == "asc" ? query.OrderBy(u => u.FirstName) : query.OrderByDescending(u => u.FirstName),
            "lastname" => request.SortOrder?.ToLower() == "asc" ? query.OrderBy(u => u.LastName) : query.OrderByDescending(u => u.LastName),
            "email" => request.SortOrder?.ToLower() == "asc" ? query.OrderBy(u => u.Email) : query.OrderByDescending(u => u.Email),
            _ => request.SortOrder?.ToLower() == "asc" ? query.OrderBy(u => u.CreatedAt) : query.OrderByDescending(u => u.CreatedAt)
        };

        var totalCount = await query.CountAsync(ct);

        var owners = await query
            .Skip((request.PageNumber - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(u => new OwnerRepresentation(
                u.Id,
                new PersonalInfo(u.FirstName, u.LastName, u.Email, u.PhoneNumber, u.AvatarFileName),
                new UserStatus(u.IsActive, u.EmailConfirmed),
                u.CreatedAt
            ))
            .ToListAsync(ct);

        return new PaginatedList<OwnerRepresentation>(owners, totalCount, request.PageNumber, request.PageSize);
    }
}
