using Request.Application.DTOs;

namespace Request.Application.Extensions;

public static class MailSubjectTypeExtensions
{
    public static string ToRequestSubject(this MailSubjectType type, int? requestId)
    {
        return type switch
        {
            MailSubjectType.LeaveRequestCreated
                => $"[HR.Portal] New leave request #{requestId} has been created",
            MailSubjectType.LeaveRequestUpdated
                => $"[HR.Portal] Leave Request #{requestId} has been updated",
            MailSubjectType.LeaveRequestApproved
                => $"[HR.Portal] Leave Request #{requestId} has been approved",
            MailSubjectType.LeaveRequestRejected
                => $"[HR.Portal] Leave Request #{requestId} has been rejected",
            MailSubjectType.LeaveRequestCancelled
                => $"[HR.Portal] Leave Request #{requestId} has been cancelled",
            MailSubjectType.LeaveRequestDeleted
                => $"[HR.Portal] Leave Request #{requestId} has been deleted",

            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}
