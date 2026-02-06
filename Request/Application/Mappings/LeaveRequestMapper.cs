using Request.Application.DTOs;
using Request.Application.ValueObjects;
using Request.Domain.Entities;
using Request.Domain.ValueObjects;

namespace Request.Application.Mappings;

public static class LeaveRequestMapper
{
    public static LeaveRequest ToEntity(CreateRequest dto)
    {
        if (!Enum.IsDefined(typeof(RequestType), dto.Type))
            throw new ArgumentOutOfRangeException(nameof(dto.Type), $"Leave Type {dto.Type} invalid.");

        var type = (RequestType)dto.Type;

        return new LeaveRequest(
            userId: dto.UserID,
            type: type,
            startDate: dto.StartDate,
            endDate: dto.EndDate,
            isHalf: dto.IsHalfDayOff,
            reason: dto.Reason
        );
    }

    public static LeaveRequest ToEntity(LeaveRequest request, UpdateRequest dto)
    {
        if (!Enum.IsDefined(typeof(RequestType), dto.Type))
            throw new ArgumentOutOfRangeException(nameof(dto.Type), $"Request Type {dto.Type} invalid.");

        if (!Enum.IsDefined(typeof(RequestStatus), dto.Status))
            throw new ArgumentOutOfRangeException(nameof(dto.Status), $"Request Status {dto.Status} invalid.");

        request.UpdateType((RequestType)dto.Type);
        request.UpdateSchedule(dto.StartDate, dto.EndDate, dto.IsHalfDayOff);
        request.UpdateReason(dto.Reason);
        request.UpdateStatus((RequestStatus)dto.Status);

        return request;
    }

    public static GetRequestResult ToViewModel(RequestResponse entity)
    {
        var res = new GetRequestResult
        {
            RequestId = entity.RequestId,
            UserID = entity.UserID,
            UserName = entity.UserName,
            Type = Enum.GetName(typeof(RequestType), entity.Type),
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsHalfDayOff = entity.IsHalfDayOff,
            Reason = entity.Reason,
            Status = Enum.GetName(typeof(RequestStatus), entity.Status),
            CreatedAt = entity.CreatedAt
        };

        return res;
    }
}