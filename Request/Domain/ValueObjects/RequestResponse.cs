using Request.Domain.Entities;

namespace Request.Domain.ValueObjects;

public sealed record RequestResponse
(
    int RequestId,
    int UserID,
    string UserName,
    string? Email,
    RequestType Type,
    DateTime? StartDate,
    DateTime? EndDate,
    bool? IsHalfDayOff,
    string? Reason,
    RequestStatus Status,
    DateTime? CreatedAt
);