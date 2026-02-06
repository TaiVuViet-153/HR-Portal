using Request.Common.Paging;

namespace Employee.Application.DTOs.Request;

public sealed class GetUserRequest : PagingQuery
{
    public string? Search { get; init; }
    public int? Status { get; init; }
    public DateTime? CreatedAfter { get; init; }
    public DateTime? CreatedBefore { get; init; }
}