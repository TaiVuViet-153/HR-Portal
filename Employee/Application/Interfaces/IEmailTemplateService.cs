namespace Employee.Application.Interfaces;

public interface IEmailTemplateService
{
    string GetUserCreatedTemplate(string userName, string email, string temporaryPassword);
    string GetUserUpdatedTemplate(string userName, string email);
    string GetUserDeletedTemplate(string userName, string email);
    string GetPasswordResetTemplate(string userName, string email, string newPassword);
    string GetAccountLockedTemplate(string userName, string email, DateTime lockedAt);
}
