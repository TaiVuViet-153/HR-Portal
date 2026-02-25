using Microsoft.EntityFrameworkCore;
using Shared.Abstractions.Paging;

namespace Shared.Infrastructures.EFCore;

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

    /// <summary>
    /// Phân trang theo key selector, sau đó fetch data và map trong memory.
    /// Dùng khi cần group by hoặc có nested collection (ToList trong projection).
    /// </summary>
    public static async Task<PagedResult<TResult>> ToPagedResultWithGroupingAsync<TSource, TKey, TResult>(
        this IQueryable<TSource> query,
        PagingQuery? paging,
        Func<TSource, TKey> keySelector,
        Func<List<TSource>, List<TResult>> mapper)
    {
        var page = paging?.Page ?? PagingQuery.DefaultPage;
        var pageSize = paging?.PageSize ?? PagingQuery.DefaultPageSize;

        if (page <= 0) page = PagingQuery.DefaultPage;
        if (pageSize <= 0) pageSize = PagingQuery.DefaultPageSize;

        // Materialize toàn bộ data (cần thiết vì keySelector là Func, không translate được)
        var allData = await query.ToListAsync();

        // Lấy distinct keys và phân trang
        var distinctKeys = allData.Select(keySelector).Distinct().ToList();
        var totalItems = distinctKeys.Count;

        var pagedKeys = distinctKeys
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToHashSet();

        // Filter data theo paged keys
        var pagedData = allData.Where(x => pagedKeys.Contains(keySelector(x))).ToList();

        // Map trong memory
        var items = mapper(pagedData);
        foreach (var item in items)
        {
            Console.WriteLine(item);
        }

        return new PagedResult<TResult>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalItems
        };
    }
}
