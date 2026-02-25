using Request.Application.DTOs;
using Request.Application.Mappings;
using Request.Application.Interfaces;
using Request.Domain.Entities;
using Request.Domain.Repositories;
using Request.Domain.ValueObjects;
using Request.Application.Extensions;
using Request.Application.DTOs.Request;
using Request.Application.Repositories;
using Shared.Abstractions.Caching;
using Shared.Abstractions.Paging;
using Request.Application.DTOs.Response;
using Shared.Abstractions.SuccessResponse;



namespace Request.Application.Services;

public class RequestService(
    ILeaveRepository leaveRepository,
    IRequestRepository requestRepository,
    IBalanceRepository balanceRepository,
    IEmailSender emailSender,
    IEmailTemplateService emailTemplateService,
    ICacheService cacheService
) : IRequestService
{
    public async Task<SuccessResponse<RequestResponse>> CreateRequest(CreateRequest newRequest)
    {
        var request = LeaveRequestMapper.ToEntity(newRequest);

        var validationResult = ValidateRequest(request);
        if (!validationResult.Success)
            return validationResult;

        if (request.Type != RequestType.Unpaid)
        {
            var balance = await GetBalanceByUser(request.UserID, request.Type);
            if (balance == null)
                return new SuccessResponse<RequestResponse>(false, "Leave balance not found");

            double leaveDays = CalculateLeaveDays(request);

            if (!balance.HasEnoughDays(leaveDays))
                return new SuccessResponse<RequestResponse>(false, "Leave balance is not enough");

            balance.DecreaseBalance(leaveDays);
            var updateBalanceResult = await leaveRepository.UpdateBalance(balance);
            if (updateBalanceResult == 0)
                return new SuccessResponse<RequestResponse>(false, "Update balance failed");
        }

        var insertResult = await leaveRepository.AddRequest(request);
        if (insertResult == 0)
            return new SuccessResponse<RequestResponse>(false, "Create request failed");

        var createdRequest = await requestRepository.GetUserByRequestId(insertResult);

        await SendEmailNotification(createdRequest);

        // Invalidate cache after successful create
        await cacheService.InvalidateByPrefixAsync(CacheKeys.LeaveRequests);


        return new SuccessResponse<RequestResponse>(true, "Create request success", createdRequest);
    }

    public async Task<PagedResult<RequestResponse>> GetRequests(GetRequestQuery query)
    {
        var cacheKey = CacheKeyBuilder.Build(query);

        var result = await cacheService.GetOrCreateAsync(
            CacheKeys.LeaveRequests,
            cacheKey,
            async () =>
            {
                return await requestRepository.GetRequests(query);
            },
            TimeSpan.FromMinutes(5));

        return result ?? new PagedResult<RequestResponse>
        {
            Items = new List<RequestResponse>(),
            Page = query.Page,
            PageSize = query.PageSize,
            TotalItems = 0
        };
    }

    public async Task<SuccessResponse<RequestResponse>> UpdateRequest(UpdateRequest updateRequest)
    {
        var existedRequest = await requestRepository.GetRequestById(updateRequest.RequestId);
        if (existedRequest == null) return new SuccessResponse<RequestResponse>(false, "Request isn't existed");

        var previousStatus = existedRequest.Status;
        var request = LeaveRequestMapper.ToEntity(existedRequest, updateRequest);

        var validationResult = ValidateRequest(request);
        if (!validationResult.Success)
            return validationResult;

        if (request.Type != RequestType.Unpaid)
        {
            var balance = await GetBalanceByUser(request.UserID, request.Type);
            if (balance == null)
                return new SuccessResponse<RequestResponse>(false, "The balance of this user is not found");

            double leaveDays = CalculateLeaveDays(request);

            if (!balance.HasEnoughDays(leaveDays) && request.Status == RequestStatus.Approved)
                return new SuccessResponse<RequestResponse>(false, "Leave balance is not enough");

            balance.DecreaseBalance(leaveDays);
            await leaveRepository.UpdateBalance(balance);
        }

        var result = await leaveRepository.UpdateRequest(request);
        if (result == 0)
            return new SuccessResponse<RequestResponse>(false, "Update request fail");

        var updatedResult = await requestRepository.GetUserByRequestId(result);

        // Send email if status changed
        if (previousStatus != request.Status)
        {
            await SendStatusChangeEmailNotification(updatedResult, request.Status, updateReason: null);
        }
        else
        {
            await SendUpdatedEmailNotification(updatedResult, updateReason: null);
        }

        // Invalidate cache after successful update
        await cacheService.InvalidateByPrefixAsync(CacheKeys.LeaveRequests);

        return new SuccessResponse<RequestResponse>(true, "Update request success", updatedResult);
    }

    public async Task<SuccessResponse<bool>> DeleteRequest(int requestId)
    {
        var existedRequest = await requestRepository.GetRequestById(requestId);
        if (existedRequest == null) return new SuccessResponse<bool>(false, "Request isn't existed");

        if (existedRequest.Status == RequestStatus.Approved)
            return new SuccessResponse<bool>(false, "Approved request can not be deleted");

        existedRequest.MarkAsDeleted();

        var result = await leaveRepository.UpdateRequest(existedRequest);
        if (result == 0)
            return new SuccessResponse<bool>(false, "Delete request fail");

        var deletedRequest = await requestRepository.GetUserByRequestId(requestId);
        await SendDeletedEmailNotification(deletedRequest, deleteReason: null);

        // Invalidate cache after successful delete
        await cacheService.InvalidateByPrefixAsync(CacheKeys.LeaveRequests);

        return new SuccessResponse<bool>(true, "Delete request success");
    }

    private async Task<LeaveBalance> GetBalanceByUser(int userId, RequestType type, int? sourceYear = null)
    {
        return await balanceRepository.GetBalanceByUser(userId, type, sourceYear);
    }

    private SuccessResponse<RequestResponse> ValidateRequest(LeaveRequest request)
    {
        if (!request.StartDate.HasValue || !request.EndDate.HasValue)
            return new SuccessResponse<RequestResponse>(false, "Start Date or End Date is invalid");

        if (request.EndDate < request.StartDate)
            return new SuccessResponse<RequestResponse>(false, "End Date must be after Start Date");

        return new SuccessResponse<RequestResponse>(true, string.Empty);
    }

    private double CalculateLeaveDays(LeaveRequest request)
    {
        double leaveDays = (request.EndDate.Value.Date - request.StartDate.Value.Date).Days + 1;
        
        DateTime start = request.StartDate.Value.Date;
        DateTime end = request.EndDate.Value.Date;
        
        for (var day = start; day <= end; day = day.AddDays(1))
        {
            if (day.DayOfWeek == DayOfWeek.Saturday || day.DayOfWeek == DayOfWeek.Sunday)
            {
                leaveDays++;
            }
        }

        return request.IsHalfDayOff.Equals(true) ? leaveDays - 0.5 : leaveDays;
    }

    private async Task SendEmailNotification(RequestResponse? createdRequest)
    {
        if (createdRequest == null) return;
    
        var emailHtml = emailTemplateService.GetLeaveRequestCreatedTemplate(
            requestId: createdRequest.RequestId,
            userName: createdRequest.UserName ?? $"User #{createdRequest.UserID}",
            leaveType: createdRequest.Type.ToString(),
            startDate: createdRequest.StartDate ?? DateTime.Now,
            endDate: createdRequest.EndDate ?? DateTime.Now,
            isHalfDay: createdRequest.IsHalfDayOff ?? false,
            reason: createdRequest.Reason
        );

        await SendEmail(
            createdRequest,
            MailSubjectType.LeaveRequestCreated,
            emailHtml
        );
    }

    private async Task SendUpdatedEmailNotification(RequestResponse? request, string? updateReason = null)
    {
        if (request == null) return;

        var emailHtml = emailTemplateService.GetLeaveRequestUpdatedTemplate(
            requestId: request.RequestId,
            userName: request.UserName ?? $"User #{request.UserID}",
            leaveType: request.Type.ToString(),
            startDate: request.StartDate ?? DateTime.Now,
            endDate: request.EndDate ?? DateTime.Now,
            isHalfDay: request.IsHalfDayOff ?? false,
            updateReason: updateReason
        );

        await SendEmail(
            request,
            MailSubjectType.LeaveRequestUpdated,
            emailHtml
        );
    }

    private async Task SendStatusChangeEmailNotification(RequestResponse? request, RequestStatus newStatus, string? updateReason = null)
    {
        if (request == null) return;

        var (mailSubjectType, emailHtml) = newStatus switch
        {
            RequestStatus.Approved => (
                MailSubjectType.LeaveRequestApproved,
                emailTemplateService.GetLeaveRequestApprovedTemplate(
                    requestId: request.RequestId,
                    userName: request.UserName ?? $"User #{request.UserID}",
                    leaveType: request.Type.ToString(),
                    startDate: request.StartDate ?? DateTime.Now,
                    endDate: request.EndDate ?? DateTime.Now,
                    isHalfDay: request.IsHalfDayOff ?? false,
                    approverName: null // Add approver name if available
                )
            ),
            RequestStatus.Rejected => (
                MailSubjectType.LeaveRequestRejected,
                emailTemplateService.GetLeaveRequestRejectedTemplate(
                    requestId: request.RequestId,
                    userName: request.UserName ?? $"User #{request.UserID}",
                    leaveType: request.Type.ToString(),
                    startDate: request.StartDate ?? DateTime.Now,
                    endDate: request.EndDate ?? DateTime.Now,
                    isHalfDay: request.IsHalfDayOff ?? false,
                    rejectReason: updateReason // Add reject reason if available
                )
            ),
            RequestStatus.Cancelled => (
                MailSubjectType.LeaveRequestCancelled,
                emailTemplateService.GetLeaveRequestCancelledTemplate(
                    requestId: request.RequestId,
                    userName: request.UserName ?? $"User #{request.UserID}",
                    leaveType: request.Type.ToString(),
                    startDate: request.StartDate ?? DateTime.Now,
                    endDate: request.EndDate ?? DateTime.Now,
                    isHalfDay: request.IsHalfDayOff ?? false,
                    cancelReason: updateReason // Add cancel reason if available
                )
            ),
            _ => (MailSubjectType.LeaveRequestCreated, string.Empty)
        };

        if (!string.IsNullOrEmpty(emailHtml))
        {
            await SendEmail(request, mailSubjectType, emailHtml);
        }
    }

    private async Task SendDeletedEmailNotification(RequestResponse? request, string? deleteReason = null)
    {
        if (request == null) return;

        var emailHtml = emailTemplateService.GetLeaveRequestDeletedTemplate(
            requestId: request.RequestId,
            userName: request.UserName ?? $"User #{request.UserID}",
            leaveType: request.Type.ToString(),
            startDate: request.StartDate ?? DateTime.Now,
            endDate: request.EndDate ?? DateTime.Now,
            isHalfDay: request.IsHalfDayOff ?? false,
            deleteReason: deleteReason
        );

        await SendEmail(request, MailSubjectType.LeaveRequestDeleted, emailHtml);
    }

    private async Task SendEmail(RequestResponse request, MailSubjectType subjectType, string emailHtml)
    {
        // No reply email address -- system email
        var noReplyEmail = "tai.vv@linhlongengineering.com";

        // Approver email address
        var recipientEmail = new[] { "tai.vv@linhlongengineering.com" };

        // Requester email address as CC
        // var ccEmail = string.IsNullOrEmpty(request.Email)
        //     ? Array.Empty<string>()
        //     : new[] { request.Email };
        var ccEmail = Array.Empty<string>();

        var message = new EmailMessage
        {
            From = noReplyEmail,
            To = recipientEmail,
            Cc = ccEmail,
            Subject = subjectType.ToRequestSubject(request.RequestId),
            Html = emailHtml
        };

        await emailSender.SendAsync(message);
    }

}