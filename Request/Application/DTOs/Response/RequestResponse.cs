using Request.Domain.Entities;
using Request.Domain.ValueObjects;

namespace Request.Application.DTOs.Response;

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