namespace Request.Application.DTOs;

public sealed record UpdateRequest
{
    public int RequestId { get; init; }
    public int UserID { get; init; }
    public byte Type { get; init; }
    public DateTime? StartDate { get; init; }
    public DateTime? EndDate { get; init; }
    public bool? IsHalfDayOff { get; init; } = false;
    public string? Reason { get; init; }
    public byte Status { get; init; }
}