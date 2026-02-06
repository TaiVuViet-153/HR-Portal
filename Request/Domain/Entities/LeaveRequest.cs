using Request.Domain.ValueObjects;

namespace Request.Domain.Entities;

public sealed class LeaveRequest
{
    public int RequestId { get; private set; }
    public int UserID { get; private set; }
    public RequestType Type { get; private set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public bool? IsHalfDayOff { get; private set; }
    public string? Reason { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public RequestStatus Status { get; set; }
    public bool IsActive { get; private set; }


    private LeaveRequest() { }
    public LeaveRequest(int userId, RequestType type, DateTime? startDate, DateTime? endDate, bool? isHalf, string? reason)
    {
        UserID = userId;
        Type = type;
        StartDate = startDate;
        EndDate = endDate;
        IsHalfDayOff = isHalf;
        Reason = reason;
        Status = RequestStatus.Pending;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }
    public void UpdateType(RequestType type)
    {
        if (!Enum.IsDefined(typeof(RequestType), type)) return;
        if (Type == type) return;

        Type = type;
        UpdateNow();
    }
    public void UpdateSchedule(DateTime? startDate, DateTime? endDate, bool? isHalf)
    {
        // Apply only when value provided and different; update timestamp only if something changed
        var changed = false;

        if (startDate.HasValue)
        {
            if (StartDate != startDate)
            {
                StartDate = startDate;
                changed = true;
            }
        }

        if (endDate.HasValue)
        {
            if (EndDate != endDate)
            {
                EndDate = endDate;
                changed = true;
            }
        }

        if (isHalf.HasValue)
        {
            if (IsHalfDayOff != isHalf)
            {
                IsHalfDayOff = isHalf;
                changed = true;
            }
        }

        if (changed) UpdateNow();
    }
    public void UpdateReason(string reason)
    {
        if (string.IsNullOrWhiteSpace(reason)) return;
        if (Reason == reason) return;

        Reason = reason;
        UpdateNow();
    }
    public void UpdateStatus(RequestStatus status)
    {
        if (!Enum.IsDefined(typeof(RequestStatus), status)) return;
        if (Status == status) return;

        Status = status;
        UpdateNow();
    }
    public void MarkAsDeleted()
    {
        IsActive = false;
        UpdateNow();
    }
    public void UpdateNow()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}