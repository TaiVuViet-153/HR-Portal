namespace Request.Common.Paging;

public abstract class PagingQuery
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public string? SortBy { get; set; }
    public int? SortDir { get; set; } = 0;
}
