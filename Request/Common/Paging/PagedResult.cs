namespace Request.Common.Paging;

public class PagedResult<T>
{
    public IReadOnlyList<T> Items { get; set; } = new List<T>();

    public int Page { get; set; }
    public int PageSize { get; set; }

    public int TotalItems { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalItems / PageSize);

    public bool HasNext => Page < TotalPages;
    public bool HasPrevious => Page > 1;
}
