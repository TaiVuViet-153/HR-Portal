namespace Shared.Abstractions.Paging;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    public int Page { get; set; }
    public int PageSize { get; set; }

    public int TotalItems { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalItems / PageSize) : 0;

    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;

    public PagedResult<TNew> WithItems<TNew>(IReadOnlyList<TNew> items)
    {
        return new PagedResult<TNew>
        {
            Items = items,
            Page = this.Page,
            PageSize = this.PageSize,
            TotalItems = this.TotalItems
        };
    }
}
