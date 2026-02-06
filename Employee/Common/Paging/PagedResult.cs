namespace Request.Common.Paging;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    public int Page { get; set; }
    public int PageSize { get; set; }

    public int TotalItems { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 0;

    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;

    public PagedResult<T> WithItems(IReadOnlyList<T> items)
    {
        return new PagedResult<T>
        {
            Items = items,
            Page = Page,
            PageSize = PageSize,
            TotalItems = TotalItems
        };
    }
}
