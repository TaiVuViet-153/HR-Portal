namespace Request.Application.Interfaces;

public interface IEmailTemplateService
{
    string GetLeaveRequestCreatedTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? reason);
    string GetLeaveRequestUpdatedTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? updateReason);

    string GetLeaveRequestApprovedTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? approverName);

    string GetLeaveRequestRejectedTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? rejectReason);

    string GetLeaveRequestCancelledTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? cancelReason);
    string GetLeaveRequestDeletedTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? deleteReason);
}
