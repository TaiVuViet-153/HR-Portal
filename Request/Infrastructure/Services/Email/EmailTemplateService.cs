using Request.Application.Interfaces;

namespace Request.Infrastructure.Services.Email;

public sealed class EmailTemplateService : IEmailTemplateService
{
    public string GetLeaveRequestCreatedTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? reason)
    {
        var durationText = GetDurationText(startDate, endDate, isHalfDay);

        return BuildEmailTemplate(
            headerIcon: "📋",
            headerTitle: "New Leave Request",
            headerGradient: "linear-gradient(135deg, #667eea 0%, #764ba2 100%)",
            greeting: "Hello,",
            message: "A new leave request has been created and is pending approval.",
            requestId: requestId,
            userName: userName,
            leaveType: leaveType,
            startDate: startDate,
            endDate: endDate,
            durationText: durationText,
            isHalfDay: isHalfDay,
            extraInfo: string.IsNullOrWhiteSpace(reason) ? null : $"Reason:|{reason}",
            statusBadge: ("⏳ Pending Approval", "#fef3c7", "#d97706"),
            borderColor: "#667eea"
        );
    }

    public string GetLeaveRequestUpdatedTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? updateReason)
    {
        var durationText = GetDurationText(startDate, endDate, isHalfDay);

        return BuildEmailTemplate(
            headerIcon: "✏️",
            headerTitle: "Leave Request Updated",
            headerGradient: "linear-gradient(135deg, #f59e0b 0%, #d97706 100%)",
            greeting: "Hello,",
            message: "The following leave request has been updated.",
            requestId: requestId,
            userName: userName,
            leaveType: leaveType,
            startDate: startDate,
            endDate: endDate,
            durationText: durationText,
            isHalfDay: isHalfDay,
            extraInfo: string.IsNullOrWhiteSpace(updateReason) ? null : $"Update Reason:|{updateReason}",
            statusBadge: ("📝 Updated", "#ffedd5", "#d97706"),
            borderColor: "#f59e0b"
        );
    }

    public string GetLeaveRequestApprovedTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? approverName)
    {
        var durationText = GetDurationText(startDate, endDate, isHalfDay);

        return BuildEmailTemplate(
            headerIcon: "✅",
            headerTitle: "Leave Request Approved",
            headerGradient: "linear-gradient(135deg, #10b981 0%, #059669 100%)",
            greeting: $"Hello {userName},",
            message: "Your leave request has been approved.",
            requestId: requestId,
            userName: userName,
            leaveType: leaveType,
            startDate: startDate,
            endDate: endDate,
            durationText: durationText,
            isHalfDay: isHalfDay,
            extraInfo: string.IsNullOrWhiteSpace(approverName) ? null : $"Approved By:|{approverName}",
            statusBadge: ("✓ Approved", "#d1fae5", "#059669"),
            borderColor: "#10b981"
        );
    }

    public string GetLeaveRequestRejectedTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? rejectReason)
    {
        var durationText = GetDurationText(startDate, endDate, isHalfDay);

        return BuildEmailTemplate(
            headerIcon: "❌",
            headerTitle: "Leave Request Rejected",
            headerGradient: "linear-gradient(135deg, #ef4444 0%, #dc2626 100%)",
            greeting: $"Hello {userName},",
            message: "Unfortunately, your leave request has been rejected.",
            requestId: requestId,
            userName: userName,
            leaveType: leaveType,
            startDate: startDate,
            endDate: endDate,
            durationText: durationText,
            isHalfDay: isHalfDay,
            extraInfo: string.IsNullOrWhiteSpace(rejectReason) ? null : $"Rejection Reason:|{rejectReason}",
            statusBadge: ("✗ Rejected", "#fee2e2", "#dc2626"),
            borderColor: "#ef4444"
        );
    }

    public string GetLeaveRequestCancelledTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? cancelReason)
    {
        var durationText = GetDurationText(startDate, endDate, isHalfDay);

        return BuildEmailTemplate(
            headerIcon: "🚫",
            headerTitle: "Leave Request Cancelled",
            headerGradient: "linear-gradient(135deg, #6b7280 0%, #4b5563 100%)",
            greeting: "Hello,",
            message: "The following leave request has been cancelled.",
            requestId: requestId,
            userName: userName,
            leaveType: leaveType,
            startDate: startDate,
            endDate: endDate,
            durationText: durationText,
            isHalfDay: isHalfDay,
            extraInfo: string.IsNullOrWhiteSpace(cancelReason) ? null : $"Cancellation Reason:|{cancelReason}",
            statusBadge: ("⊘ Cancelled", "#f3f4f6", "#6b7280"),
            borderColor: "#6b7280"
        );
    }
    public string GetLeaveRequestDeletedTemplate(
        int requestId,
        string userName,
        string leaveType,
        DateTime startDate,
        DateTime endDate,
        bool isHalfDay,
        string? deleteReason)
    {
        var durationText = GetDurationText(startDate, endDate, isHalfDay);

        return BuildEmailTemplate(
            headerIcon: "🗑️",
            headerTitle: "Leave Request Deleted",
            headerGradient: "linear-gradient(135deg, #9ca3af 0%, #6b7280 100%)",
            greeting: "Hello,",
            message: "The following leave request has been deleted.",
            requestId: requestId,
            userName: userName,
            leaveType: leaveType,
            startDate: startDate,
            endDate: endDate,
            durationText: durationText,
            isHalfDay: isHalfDay,
            extraInfo: string.IsNullOrWhiteSpace(deleteReason) ? null : $"Deletion Reason:|{deleteReason}",
            statusBadge: ("🗑️ Deleted", "#e5e7eb", "#6b7280"),
            borderColor: "#9ca3af"
        );
    }

    private static string GetDurationText(DateTime startDate, DateTime endDate, bool isHalfDay)
    {
        var duration = (endDate.Date - startDate.Date).Days + 1;
        return isHalfDay ? $"{duration - 0.5} days" : $"{duration} days";
    }

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
