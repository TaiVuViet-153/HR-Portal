using System.Linq.Expressions;
using System.Linq.Dynamic.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace Request.Common.Paging;

public static class IQueryableExtensions
{
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PagingQuery? paging)
    {
        var page = paging?.Page ?? PagingQuery.DefaultPage;
        var pageSize = paging?.PageSize ?? PagingQuery.DefaultPageSize;

        if (page <= 0) page = PagingQuery.DefaultPage;
        if (pageSize <= 0) pageSize = PagingQuery.DefaultPageSize;

        var totalItems = await query.CountAsync();

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
        Console.WriteLine($"Paging - Page: {page}, PageSize: {pageSize}, TotalItems: {totalItems}, ItemsFetched: {items.Count}");
        return new PagedResult<T>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }

    public static PagedResult<T> WithItems<T>(
        this PagedResult<T> pagedResult,
        IReadOnlyList<T> items
    )
    {
        return new PagedResult<T>
        {
            Items = items,
            Page = pagedResult.Page,
            PageSize = pagedResult.PageSize,
            TotalItems = pagedResult.TotalItems
        };
    }
}
