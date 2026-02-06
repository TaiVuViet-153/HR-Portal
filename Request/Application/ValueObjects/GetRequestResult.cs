using Request.Domain.Entities;

namespace Request.Application.ValueObjects;

public sealed record GetRequestResult()
{
    public int RequestId { get; init; }
    public int UserID { get; init; }
    public string UserName { get; init; }
    public string Type { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool? IsHalfDayOff { get; init; }
    public string? Reason { get; init; }
    public DateTime? CreatedAt { get; init; }
    public string Status { get; init; }
}