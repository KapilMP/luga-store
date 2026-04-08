using Microsoft.EntityFrameworkCore;

namespace LugaStore.Application.Common.Models;

public static class PaginatedExtensions
{
    public static Task<PaginatedList<T>> PaginatedListAsync<T>(this IQueryable<T> queryable, int pageNumber, int pageSize, CancellationToken ct = default)
        where T : class
        => PaginatedList<T>.CreateAsync(queryable, pageNumber, pageSize, ct);
}
