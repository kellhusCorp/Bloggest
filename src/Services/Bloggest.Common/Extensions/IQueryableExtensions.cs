using Microsoft.EntityFrameworkCore;

namespace Bloggest.Common.Extensions;

public static class QueryableExtensions
{
    public static async Task<PagedResult<TDestination>> GetPagedAsync<TDestination>(
        this IQueryable<TDestination> query,
        PaginationOptions options)
    {
        var count = await query.CountAsync();

        var result = await query
            .Skip(options.Offset)
            .Take(options.Limit)
            .ToArrayAsync();

        return new PagedResult<TDestination>(result, count);
    }
}