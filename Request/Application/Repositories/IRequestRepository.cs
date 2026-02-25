using Request.Application.DTOs.Request;
using Request.Application.DTOs.Response;
using Request.Domain.Entities;

using Shared.Abstractions.Paging;

namespace Request.Application.Repositories;

public interface IRequestRepository
{
    Task<PagedResult<RequestResponse>> GetRequests(GetRequestQuery query);
    public Task<LeaveRequest?> GetRequestById(int requestId);
    public Task<RequestResponse?> GetUserByRequestId(int requestId);

}