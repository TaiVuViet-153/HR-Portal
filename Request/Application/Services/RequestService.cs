using System.ComponentModel.DataAnnotations;
using Microsoft.VisualBasic;
using Request.Application.DTOs;
using Request.Application.Mappings;
using Request.Application.Interfaces;
using Request.Application.ValueObjects;
using Request.Common.Paging;
using Request.Domain.Entities;
using Request.Domain.Repositories;
using Request.Domain.ValueObjects;
using Request.Application.Extensions;

namespace Request.Application.Services;

public class RequestService(
    ILeaveRepository leaveRepository,
    IEmailSender emailSender,
    IEmailTemplateService emailTemplateService
) : IRequestService
{
    public async Task<RequestResult> CreateRequest(CreateRequest newRequest)
    {
        var request = LeaveRequestMapper.ToEntity(newRequest);

        var validationResult = ValidateRequest(request);
        if (!validationResult.success)
            return validationResult;

        if (request.Type != RequestType.Unpaid)
        {
            var balance = await GetBalanceByUser(request.UserID, request.Type);
            if (balance == null)
                return new RequestResult(false, "Leave balance not found");

            double leaveDays = CalculateLeaveDays(request);

            if (!balance.HasEnoughDays(leaveDays))
                return new RequestResult(false, "Leave balance is not enough");

            balance.UpdateBalance(leaveDays);
            var updateBalanceResult = await leaveRepository.UpdateBalance(balance);
            if (updateBalanceResult == 0)
                return new RequestResult(false, "Update balance failed");
        }

        var insertResult = await leaveRepository.AddRequest(request);
        if (insertResult == 0)
            return new RequestResult(false, "Create request failed");

        var createdRequest = leaveRepository.GetUserNameForRequest(insertResult);

        await SendEmailNotification(createdRequest);

        return new RequestResult(true, "Create request success", LeaveRequestMapper.ToViewModel(createdRequest));
    }

    public async Task<PagedResult<GetRequestResult>> GetRequests(GetRequestQuery query)
    {
        var data = leaveRepository.GetRequests();

        data = FilteredRequest(data, query);

        var pagedEntities = await data.ToPagedResultAsync(query.Page, query.PageSize);

        var mapped = pagedEntities.Items.Select(item =>
        {
            var request = leaveRepository.GetUserNameForRequest(item.RequestId);
            return LeaveRequestMapper.ToViewModel(request);
        }).ToList();

        mapped = SortMappedRequests(mapped, query).ToList();

        // ========== PAGING ==========
        return new PagedResult<GetRequestResult>
        {
            Items = mapped,
            Page = pagedEntities.Page,
            PageSize = pagedEntities.PageSize,
            TotalItems = pagedEntities.TotalItems
        };
    }
    public async Task<RequestResult> UpdateRequest(UpdateRequest updateRequest)
    {
        var existedRequest = await leaveRepository.GetByRequestId(updateRequest.RequestId);
        if (existedRequest == null) return new RequestResult(false, "Request isn't existed");

        var previousStatus = existedRequest.Status;
        var request = LeaveRequestMapper.ToEntity(existedRequest, updateRequest);

        var validationResult = ValidateRequest(request);
        if (!validationResult.success)
            return validationResult;

        if (request.Type != RequestType.Unpaid)
        {
            var balance = await GetBalanceByUser(request.UserID, request.Type);
            if (balance == null)
                return new RequestResult(false, "The balance of this user is not found");

            double leaveDays = CalculateLeaveDays(request);

            if (!balance.HasEnoughDays(leaveDays))
                return new RequestResult(false, "Leave balance is not enough");

            balance.UpdateBalance(leaveDays);
            await leaveRepository.UpdateBalance(balance);
        }

        var result = await leaveRepository.UpdateRequest(request);
        if (result == 0)
            return new RequestResult(false, "Update request fail");

        var updatedResult = leaveRepository.GetUserNameForRequest(result);

        // Send email if status changed
        if (previousStatus != request.Status)
        {
            await SendStatusChangeEmailNotification(updatedResult, request.Status, updateReason: null);
        }
        else
        {
            await SendUpdatedEmailNotification(updatedResult, updateReason: null);
        }

        return new RequestResult(true, "Update request success", LeaveRequestMapper.ToViewModel(updatedResult));
    }
    public async Task<RequestResult> DeleteRequest(int requestId)
    {
        var existedRequest = await leaveRepository.GetByRequestId(requestId);
        if (existedRequest == null) return new RequestResult(false, "Request isn't existed");

        if (existedRequest.Status == RequestStatus.Approved)
            return new RequestResult(false, "Approved request can not be deleted");

        existedRequest.MarkAsDeleted();

        var result = await leaveRepository.UpdateRequest(existedRequest);
        if (result == 0)
            return new RequestResult(false, "Delete request fail");

        var deletedRequest = leaveRepository.GetUserNameForRequest(requestId);
        await SendDeletedEmailNotification(deletedRequest, deleteReason: null);

        return new RequestResult(true, "Delete request success");
    }
    private async Task<LeaveBalance> GetBalanceByUser(int userId, RequestType type)
    {
        return await leaveRepository.GetBalanceByUser(userId, type);
    }
    private RequestResult ValidateRequest(LeaveRequest request)
    {
        if (!request.StartDate.HasValue || !request.EndDate.HasValue)
            return new RequestResult(false, "Start Date or End Date is invalid");

        if (request.EndDate < request.StartDate)
            return new RequestResult(false, "End Date must be after Start Date");

        return new RequestResult(true, string.Empty);
    }
    private double CalculateLeaveDays(LeaveRequest request)
    {
        double leaveDays = (request.EndDate.Value.Date - request.StartDate.Value.Date).Days + 1;

        return request.IsHalfDayOff.Equals(true) ? leaveDays - 0.5 : leaveDays;
    }
    private IEnumerable<GetRequestResult> SortMappedRequests(IEnumerable<GetRequestResult> items, GetRequestQuery query)
    {
        var sortBy = string.IsNullOrWhiteSpace(query.SortBy) ? "CreatedAt" : query.SortBy;
        var ascending = query.SortDir == 1;

        return sortBy switch
        {
            "RequestId" => ascending ? items.OrderBy(x => x.RequestId) : items.OrderByDescending(x => x.RequestId),
            "Type" => ascending ? items.OrderBy(x => x.Type) : items.OrderByDescending(x => x.Type),
            "StartDate" => ascending ? items.OrderBy(x => x.StartDate) : items.OrderByDescending(x => x.StartDate),
            "EndDate" => ascending ? items.OrderBy(x => x.EndDate) : items.OrderByDescending(x => x.EndDate),
            _ => ascending ? items.OrderBy(x => x.CreatedAt) : items.OrderByDescending(x => x.CreatedAt)
        };
    }
    private IQueryable<LeaveRequest> FilteredRequest(IQueryable<LeaveRequest> data, GetRequestQuery query)
    {
        // ========== FILTER ==========
        if (query.UserID.HasValue)
            data = data.Where(x => x.UserID == query.UserID.Value);

        if (query.Type.HasValue)
            data = data.Where(x => (RequestType)x.Type == (RequestType)query.Type.Value);

        if (query.StartDate.HasValue)
            data = data.Where(x => x.StartDate >= query.StartDate.Value);

        if (query.EndDate.HasValue)
            data = data.Where(x => x.EndDate <= query.EndDate.Value);

        if (query.IsHalfDayOff.HasValue)
            data = data.Where(x => x.IsHalfDayOff == query.IsHalfDayOff);

        if (!string.IsNullOrWhiteSpace(query.Reason))
            data = data.Where(x => x.Reason != null && x.Reason.Contains(query.Reason));

        if (query.Status.HasValue)
            data = data.Where(x => (RequestStatus)x.Status == (RequestStatus)query.Status.Value);

        return data;
    }
    private async Task SendEmailNotification(RequestResponse createdRequest)
    {
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
    private async Task SendUpdatedEmailNotification(RequestResponse request, string? updateReason = null)
    {
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
    private async Task SendStatusChangeEmailNotification(RequestResponse request, RequestStatus newStatus, string? updateReason = null)
    {
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

    private async Task SendDeletedEmailNotification(RequestResponse request, string? deleteReason = null)
    {
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