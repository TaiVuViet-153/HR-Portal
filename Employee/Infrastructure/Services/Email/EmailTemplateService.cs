using Employee.Application.Interfaces;

namespace Employee.Infrastructure.Services.Email;

public sealed class EmailTemplateService : IEmailTemplateService
{
    public string GetAccountLockedTemplate(string userName, string email, DateTime lockedAt)
    {
        return BuildSimpleEmailTemplate(
            headerIcon: "🔒",
            headerTitle: "Account Locked",
            headerGradient: "linear-gradient(135deg, #dc2626 0%, #b91c1c 100%)",
            greeting: $"Hello {userName},",
            message: "Your account has been locked by Admin. Please contact your administrator to unlock your account.",
            details: new Dictionary<string, string>
            {
                { "Account", email },
                { "Locked At", lockedAt.ToString("MM/dd/yyyy HH:mm:ss") },
                { "Reason", "Locked by Admin" }
            },
            borderColor: "#dc2626",
            footerNote: "If you did not attempt to log in, please contact your administrator immediately."
        );
    }

    public string GetPasswordResetTemplate(string userName, string email, string newPassword)
    {
        return BuildSimpleEmailTemplate(
            headerIcon: "🔑",
            headerTitle: "Password Reset",
            headerGradient: "linear-gradient(135deg, #f59e0b 0%, #d97706 100%)",
            greeting: $"Hello {userName},",
            message: "Your password has been reset by an administrator. Please use the temporary password below to log in and change your password immediately.",
            details: new Dictionary<string, string>
            {
                { "Account", email },
                { "Temporary Password", newPassword },
                { "Reset At", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") }
            },
            borderColor: "#f59e0b",
            footerNote: "For security reasons, please change your password after logging in."
        );
    }

    public string GetUserCreatedTemplate(string userName, string email, string temporaryPassword)
    {
        return BuildSimpleEmailTemplate(
            headerIcon: "🎉",
            headerTitle: "Welcome to HR Portal",
            headerGradient: "linear-gradient(135deg, #10b981 0%, #059669 100%)",
            greeting: $"Hello {userName},",
            message: "Your account has been successfully created. Please use the credentials below to log in to the HR Portal.",
            details: new Dictionary<string, string>
            {
                { "Username", email },
                { "Temporary Password", temporaryPassword },
                { "Created At", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") }
            },
            borderColor: "#10b981",
            footerNote: "Please change your password after your first login for security purposes."
        );
    }

    public string GetUserDeletedTemplate(string userName, string email)
    {
        return BuildSimpleEmailTemplate(
            headerIcon: "👋",
            headerTitle: "Account Deleted",
            headerGradient: "linear-gradient(135deg, #6b7280 0%, #4b5563 100%)",
            greeting: $"Hello {userName},",
            message: "Your HR Portal account has been deleted. You will no longer have access to the system.",
            details: new Dictionary<string, string>
            {
                { "Account", email },
                { "Deleted At", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") }
            },
            borderColor: "#6b7280",
            footerNote: "If you believe this was done in error, please contact your administrator."
        );
    }

    public string GetUserUpdatedTemplate(string userName, string email)
    {

        return BuildSimpleEmailTemplate(
            headerIcon: "✏️",
            headerTitle: "Profile Updated",
            headerGradient: "linear-gradient(135deg, #3b82f6 0%, #2563eb 100%)",
            greeting: $"Hello {userName},",
            message: "Your profile information has been updated in the HR Portal.",
            details: new Dictionary<string, string>
            {
                { "Account", email },
                { "Updated At", DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss") }
            },
            borderColor: "#3b82f6",
            footerNote: "If you did not make these changes, please contact your administrator."
        );
    }

    private static string BuildSimpleEmailTemplate(
        string headerIcon,
        string headerTitle,
        string headerGradient,
        string greeting,
        string message,
        Dictionary<string, string> details,
        string borderColor,
        string footerNote)
    {
        var detailsHtml = string.Join("", details.Select((kvp, index) => $"""
            <tr>
                <td style="padding: 12px 0;{(index > 0 ? " border-top: 1px solid #e5e7eb;" : "")}">
                    <span style="color: #6b7280; font-size: 14px;">{kvp.Key}:</span>
                    <span style="color: #1f2937; font-size: 14px; font-weight: 600; float: right;">{kvp.Value}</span>
                </td>
            </tr>
        """));

        return $"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
        </head>
        <body style="margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7fa;">
            <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);">
                <tr>
                    <td style="background: {headerGradient}; padding: 30px 40px; border-radius: 12px 12px 0 0;">
                        <h1 style="color: #ffffff; margin: 0; font-size: 24px; font-weight: 600;">{headerIcon} {headerTitle}</h1>
                    </td>
                </tr>
                <tr>
                    <td style="padding: 40px;">
                        <p style="color: #374151; font-size: 16px; line-height: 1.6; margin: 0 0 20px;">{greeting}</p>
                        <p style="color: #374151; font-size: 16px; line-height: 1.6; margin: 0 0 30px;">{message}</p>
                        <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color: #f8fafc; border-radius: 8px; border-left: 4px solid {borderColor};">
                            <tr>
                                <td style="padding: 24px;">
                                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0">
                                        {detailsHtml}
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <p style="color: #6b7280; font-size: 14px; margin: 24px 0 0; line-height: 1.6; font-style: italic;">{footerNote}</p>
                    </td>
                </tr>
                <tr>
                    <td style="background-color: #f8fafc; padding: 24px 40px; border-radius: 0 0 12px 12px; border-top: 1px solid #e5e7eb;">
                        <p style="color: #6b7280; font-size: 13px; margin: 0; text-align: center; line-height: 1.6;">This email was sent automatically from the HR Portal system.<br>Please do not reply to this email.</p>
                        <p style="color: #9ca3af; font-size: 12px; margin: 16px 0 0; text-align: center;">© {DateTime.Now.Year} HR Portal. All rights reserved.</p>
                    </td>
                </tr>
            </table>
        </body>
        </html>
        """;
    }

    // Keep existing BuildEmailTemplate for leave-related emails
    private static string BuildEmailTemplate(
        string headerIcon,
        string headerTitle,
        string headerGradient,
        string greeting,
        string message,
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        string durationText,
        bool isHalfDay,
        string? extraInfo,
        (string text, string bgColor, string textColor) statusBadge,
        string borderColor)
    {
        var extraInfoHtml = "";
        if (!string.IsNullOrEmpty(extraInfo))
        {
            var parts = extraInfo.Split('|');
            extraInfoHtml = $"""
                <tr>
                    <td style="padding: 8px 0; border-top: 1px solid #e5e7eb;">
                        <span style="color: #6b7280; font-size: 14px;">{parts[0]}</span>
                        <p style="color: #1f2937; font-size: 14px; margin: 8px 0 0; line-height: 1.5;">{parts[1]}</p>
                    </td>
                </tr>
            """;
        }

        return $"""
        <!DOCTYPE html>
        <html lang="en">
        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
        </head>
        <body style="margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f4f7fa;">
            <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="max-width: 600px; margin: 20px auto; background-color: #ffffff; border-radius: 12px; box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);">
                <tr>
                    <td style="background: {headerGradient}; padding: 30px 40px; border-radius: 12px 12px 0 0;">
                        <h1 style="color: #ffffff; margin: 0; font-size: 24px; font-weight: 600;">{headerIcon} {headerTitle}</h1>
                    </td>
                </tr>
                <tr>
                    <td style="padding: 40px;">
                        <p style="color: #374151; font-size: 16px; line-height: 1.6; margin: 0 0 20px;">{greeting}</p>
                        <p style="color: #374151; font-size: 16px; line-height: 1.6; margin: 0 0 30px;">{message}</p>
                        <table role="presentation" width="100%" cellspacing="0" cellpadding="0" style="background-color: #f8fafc; border-radius: 8px; border-left: 4px solid {borderColor};">
                            <tr>
                                <td style="padding: 24px;">
                                    <table role="presentation" width="100%" cellspacing="0" cellpadding="0">
                                        <tr><td style="padding: 8px 0;"><span style="color: #6b7280; font-size: 14px;">Request ID:</span><span style="color: #1f2937; font-size: 14px; font-weight: 600; float: right;">#{requestId}</span></td></tr>
                                        <tr><td style="padding: 8px 0; border-top: 1px solid #e5e7eb;"><span style="color: #6b7280; font-size: 14px;">Employee:</span><span style="color: #1f2937; font-size: 14px; font-weight: 600; float: right;">{userName}</span></td></tr>
                                        <tr><td style="padding: 8px 0; border-top: 1px solid #e5e7eb;"><span style="color: #6b7280; font-size: 14px;">Leave Type:</span><span style="color: {borderColor}; font-size: 14px; font-weight: 600; float: right;">{leaveType}</span></td></tr>
                                        <tr><td style="padding: 8px 0; border-top: 1px solid #e5e7eb;"><span style="color: #6b7280; font-size: 14px;">From Date:</span><span style="color: #1f2937; font-size: 14px; font-weight: 600; float: right;">{startDate:MM/dd/yyyy}</span></td></tr>
                                        <tr><td style="padding: 8px 0; border-top: 1px solid #e5e7eb;"><span style="color: #6b7280; font-size: 14px;">To Date:</span><span style="color: #1f2937; font-size: 14px; font-weight: 600; float: right;">{endDate:MM/dd/yyyy}</span></td></tr>
                                        <tr><td style="padding: 8px 0; border-top: 1px solid #e5e7eb;"><span style="color: #6b7280; font-size: 14px;">Duration:</span><span style="color: #1f2937; font-size: 14px; font-weight: 600; float: right;">{durationText}{(isHalfDay ? " (Half day)" : "")}</span></td></tr>
                                        {extraInfoHtml}
                                    </table>
                                </td>
                            </tr>
                        </table>
                        <div style="margin-top: 24px; text-align: center;">
                            <span style="display: inline-block; background-color: {statusBadge.bgColor}; color: {statusBadge.textColor}; padding: 8px 16px; border-radius: 20px; font-size: 14px; font-weight: 500;">{statusBadge.text}</span>
                        </div>
                        
                    </td>
                </tr>
                <tr>
                    <td style="background-color: #f8fafc; padding: 24px 40px; border-radius: 0 0 12px 12px; border-top: 1px solid #e5e7eb;">
                        <p style="color: #6b7280; font-size: 13px; margin: 0; text-align: center; line-height: 1.6;">This email was sent automatically from the HR Portal system.<br>Please do not reply to this email.</p>
                        <p style="color: #9ca3af; font-size: 12px; margin: 16px 0 0; text-align: center;">© {DateTime.Now.Year} HR Portal. All rights reserved.</p>
                    </td>
                </tr>
            </table>
        </body>
        </html>
        """;
    }
}
