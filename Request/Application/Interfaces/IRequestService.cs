using Request.Application.DTOs;
using Request.Application.DTOs.Request;
using Request.Application.DTOs.Response;
using Shared.Abstractions.Paging;
using Shared.Abstractions.SuccessResponse;

namespace Request.Application.Interfaces;

public interface IRequestService
{
    Task<SuccessResponse<RequestResponse>> CreateRequest(CreateRequest newRequest);
    Task<PagedResult<RequestResponse>> GetRequests(GetRequestQuery query);
    Task<SuccessResponse<RequestResponse>> UpdateRequest(UpdateRequest updatedRequest);
    Task<SuccessResponse<bool>> DeleteRequest(int requestId);
}