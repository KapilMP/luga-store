namespace SedaWears.Application.Common.Models;

public record PaginatedResult<T>(
    List<T> Items,
    int TotalCount,
    int PageNumber,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage);

public static class PaginatedResultExtensions
{
    public static PaginatedResult<T> ToPaginatedResult<T>(this List<T> items, int totalCount, int pageNumber, int pageSize)
    {
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        return new PaginatedResult<T>(
            items,
            totalCount,
            pageNumber,
            totalPages,
            pageNumber > 1,
            pageNumber < totalPages);
    }
}
