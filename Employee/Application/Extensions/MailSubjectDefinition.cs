using Employee.Application.DTOs;

namespace Employee.Application.Extensions;

public static class MailSubjectTypeExtensions
{
    public static string ToSubject(this MailSubjectType type, string? userName)
    {
        return type switch
        {
            MailSubjectType.UserCreated
                => $"[HR.Portal] New user {userName} has been created",
            MailSubjectType.UserUpdated
                => $"[HR.Portal] User {userName} has been updated",
            MailSubjectType.UserDeleted
                => $"[HR.Portal] User {userName} has been deleted",
            MailSubjectType.PasswordReset
                => $"[HR.Portal] Password reset for user {userName}",
            MailSubjectType.AccountLocked
                => $"[HR.Portal] Account locked for user {userName}",

            _ => throw new ArgumentOutOfRangeException(nameof(type))
        };
    }
}
