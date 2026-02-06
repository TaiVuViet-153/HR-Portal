namespace Request.Common.Paging;

public abstract class PagingQuery
{
    public const int DefaultPage = 1;
    public const int DefaultPageSize = 20;
    public const string DefaultSortBy = "CreatedAt";

    public int Page { get; set; } = DefaultPage;
    public int PageSize { get; set; } = DefaultPageSize;

    public string? SortBy { get; set; }
    public int SortDir { get; set; } = 0; // 0 = Descending, 1 = Ascending

    public bool IsAscending => SortDir == 1;
}
