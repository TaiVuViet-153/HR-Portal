using Request.Common.Paging;

namespace Request.Application.DTOs;

public sealed class GetRequestQuery : PagingQuery
{
    public int? UserID { get; init; }
    public byte? Type { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool? IsHalfDayOff { get; init; }
    public string? Reason { get; init; }
    public DateTime? CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public byte? Status { get; init; }
}
